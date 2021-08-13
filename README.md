# ml.net-object-detection using YOLOv4

This is a fork of the repo by Andreas Petersson (wootapa on GitHub), re-purposed for demo of ml.net with YOLOv4.

To get this to smoothly run on Debian, one needs to install:

```
apt-get update -y && apt-get install -y --no-install-recommends &#92;\
libgomp1 &#92;\
libgdiplus
```

For MacOS the brew can get you:
```
brew install libomp
brew install mono-libgdiplus
```