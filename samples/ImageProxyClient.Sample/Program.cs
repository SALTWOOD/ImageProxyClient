using ImageProxyClient;

var client = new ImgproxyClient(
    baseUrl: "http://192.168.0.243:3074",
    hexKey: "518d5b54256030d4ec2d1fc890e747a4",
    hexSalt: "d957e632cfaeb95f699675b77323a118"
);

// Advanced usage: more processing options
var url = client.BuildUrl("s3://dress/米露/IMG_20241213_235734.jpg", options =>
    options.GravitySmart()
          .Format("webp")
          .Quality(100)
          .StripMetadata());

Console.WriteLine($"Image URL: {url}");