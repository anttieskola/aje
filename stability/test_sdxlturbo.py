#!/home/projects/aje/stability/ai/bin/python3
from diffusers import AutoPipelineForText2Image
import torch

pipe = AutoPipelineForText2Image.from_pretrained("/home/models/sdxl-turbo", torch_dtype=torch.float16, variant="fp16")
pipe.to("cuda")

prompt = "Santa claus and pope playing disc golf. Cartoonish style."
image = pipe(prompt=prompt, num_inference_steps=1, guidance_scale=0.0).images[0]
image.save('/home/antti/Pictures/Stability/disc_golf.png')
