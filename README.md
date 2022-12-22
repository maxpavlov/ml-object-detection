[![Stand With Ukraine](https://raw.githubusercontent.com/vshymanskyy/StandWithUkraine/main/banner2-direct.svg)](https://stand-with-ukraine.pp.ua)

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

This solution also relies on YOLOv4 onnx model, thus download: https://github.com/onnx/models/tree/master/vision/object_detection_segmentation/yolov4/model
and place `yolov4.onnx` within the `/temp/` directory in the `mlnet-object-detection-demo` project. Since ist's a large file, op out from committing it to source control if you decide to fork this repo. 
