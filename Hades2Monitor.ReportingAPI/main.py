from fastapi import FastAPI

app = FastAPI()

@app.get("/")
async def root() :
    return {"message": "Hello World"}

@app.get("/memory-ram/{computerName}")
async def get_memory_ram(computerName: str) :
    return {"message": "MemoryRAM data of single computer"}

@app.get("/procesor-cpu/{computerName}")
async def get_procesor_cpu(computerName: str) :
    return {"message": "ProcesorCPU data of single computer"}

