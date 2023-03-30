# Benchmark for Unity Network Frameworks

Adapted from mirror example: 

https://mirror-networking.gitbook.io/docs/manual/examples/multiple-additive-scenes

## Mirror
https://mirror-networking.gitbook.io/

## Fishnet
https://github.com/FirstGearGames/FishNet

## Run

### Server
```bash
./benchmark.x86_64 s 100
```

### Headless Client
```bash
./benchmark.x86_64 c 127.0.0.1 true
```

### GUI Client
Just launch the unity player

## Result

Server: AWS t3.xlarge

Client: 3 x AWS t3.xlarge, 33 headless process launched per node

Use one GUI Client to observe.

- mirror: [mirror](./result/mirror_result.mp4)
- fishnet: [fishnet](./result/fishnet_result.mp4)

### Bandwidth Usage
- Mirror: 140 Mbps
- Fishnet: 70 Mbps

### Server Sync Rate
Both: 60hz


