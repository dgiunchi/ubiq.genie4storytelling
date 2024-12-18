import torch
from PIL import Image
from diffusers import StableDiffusionPipeline
from diffusers import EulerDiscreteScheduler #sampler can be changed

import hashlib
import os
import sys
import json
import argparse


parser = argparse.ArgumentParser()
parser.add_argument("--model", type=str, default="runwayml/stable-diffusion-v1-5")
parser.add_argument("--output_folder", type=str, default="./output")
parser.add_argument("--prompt_postfix", type=str, default="")
parser.add_argument("--lora_path", type=str, default="")
parser.add_argument("--seamless", type=bool, default=False  )
args = parser.parse_args()

if args.seamless:
    # Patch torch.nn.Conv2d to create seamless textures (https://github.com/huggingface/diffusers/issues/556#issuecomment-1253772217)
    def patch_conv(cls):
        init = cls.__init__

        def __init__(self, *args, **kwargs):
            return init(self, *args, **kwargs, padding_mode="circular")

        cls.__init__ = __init__

    patch_conv(torch.nn.Conv2d)

queue = []
pipe = None
generator = None
busy = False
done = False


def generateTextureFromPrompt(pipe, generator, message):
    global busy
    message = message.decode()
    if message != "" and busy == False:
        busy = True
        # Get dict from JSON string in prompt
        message = json.loads(message)
        prompt = message["prompt"]
        file_name = message["output_file"] + ".png"

        prompt += args.prompt_postfix
        image = pipe(prompt, guidance_scale=7, num_inference_steps=28, generator=generator).images[0]
        fullpath = os.path.join(args.output_folder, file_name)
        image.save(fullpath)
        print(file_name)
        busy = False


def recognize_from_stdin():
    global pipe, generator, done, busy

    if pipe == None:
        """
        print(
            "Initializing texture generation model "
            + args.model
            + " (output folder: "
            + args.output_folder
            + "."
        )
        {"prompt":"young girl eating {emma:1} an apple", "output_file": "test.png"}
        """
            
        pipe = StableDiffusionPipeline.from_pretrained( args.model, revision="fp16", torch_dtype=torch.float16)
        #pipe = StableDiffusionPipeline.from_ckpt("https://huggingface.co/runwayml/stable-diffusion-v1-5/blob/main/v1-5-pruned-emaonly.safetensors", torch_dtype=torch.float16, use_safetensors=True)
        pipe.to("cuda")
        
        if(args.lora_path != ""):
            pipe.load_lora_weights(args.lora_path)
            pipe.scheduler = EulerDiscreteScheduler.from_config(pipe.scheduler.config)
        generator = torch.Generator("cuda").manual_seed(1024)
        
    # Write stdin to the stream
    while not done:
        try:
            line = sys.stdin.buffer.readline()
            if len(line) == 0:
                continue
            generateTextureFromPrompt(pipe, generator, line)
        except KeyboardInterrupt:
            break


recognize_from_stdin()
