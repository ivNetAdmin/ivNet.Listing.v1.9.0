
using System;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Drawing;
using ivNet.Listing.Entities;
using Image = System.Drawing.Image;

namespace ivNet.Listing.Helpers
{
    public static class ImageHelper
    {        
        public static void UploadImage(HttpPostedFileBase file, string fileName, string filePath, int width, int height)
        {
            var bitmapImage = Image.FromStream(file.InputStream, true, true);
           
            // Encoder parameter for image quality
            var qualityParam = new EncoderParameter(Encoder.Quality, 100);

            // Jpeg image codec
            var jpegCodec = ImageCodecInfo.GetImageEncoders().First(codecInfo => codecInfo.MimeType == "image/jpeg");

            if (jpegCodec == null)
                return;

            using (var encoderParams = new EncoderParameters(1))
            {
                encoderParams.Param[0] = qualityParam;
               
                var isExists = Directory.Exists(filePath);
                if (!isExists)
                {
                    Directory.CreateDirectory(filePath);
                }

                var fullPath = string.Format("{0}\\{1}", filePath, fileName);

                //var imageHeight = bitmapImage.Height;

                //var imageWidth = bitmapImage.Width;

                Rectangle rectangle;

                // three conditions long rect, tall rect and square
                if (bitmapImage.Width > bitmapImage.Height)
                {
                    //long rect
                    bitmapImage = SetHeight(bitmapImage, height);
                   
                    if (bitmapImage.Width >= width)
                    {
                        var x = (bitmapImage.Width - width)/2;
                        rectangle = new Rectangle(x, 0, Convert.ToInt32(width)-1, Convert.ToInt32(height)-1);
                       
                    }
                    else
                    {
                        bitmapImage = SetWidth(bitmapImage, width);
                        var y = (bitmapImage.Height - height) / 2;
                        rectangle = new Rectangle(0, y, Convert.ToInt32(width)-1, Convert.ToInt32(height)-1);
                    }
                }else if (bitmapImage.Height > bitmapImage.Width)
                {
                    //tall rect
                    bitmapImage = SetWidth(bitmapImage, width);

                    if (bitmapImage.Height >= height)
                    {
                        var y = (bitmapImage.Height - height)/2;
                        rectangle = new Rectangle(0, y, Convert.ToInt32(width)-1, Convert.ToInt32(height)-1);
                    }
                    else
                    {
                        bitmapImage = SetHeight(bitmapImage, width);
                        var x = (bitmapImage.Width - width)/2;
                        rectangle = new Rectangle(x, 0, Convert.ToInt32(width)-1, Convert.ToInt32(height)-1);
                    }
                }
                else
                {
                    // square
                    bitmapImage = SetWidth(bitmapImage, width);
                    var y = (bitmapImage.Height - height) / 2;
                    rectangle = new Rectangle(0, y, Convert.ToInt32(width)-1, Convert.ToInt32(height)-1);
                }

                var cropImage = CropImage(bitmapImage, rectangle);
                cropImage.Save(fullPath);

                //var resizeWidth = (resizeHeight*imageWidth/imageHeight) - 1;

                //var resizeImage = ResizeImage(bitmapImage, new Size(resizeWidth, resizeHeight));

                // aspect ratioo : 1 x 1.84

                //if (resizeWidth > width)
                //{                 
                //    var x = (resizeWidth - width) / 2;
                //    var rectangle = new Rectangle(x, 0, Convert.ToInt32(cropWidth), Convert.ToInt32(cropheight));
                //    var cropImage = CropImage(resizeImage, rectangle);
                //    cropImage.Save(fullPath);
                //}
                //else
                //{
                //    if (resizeImage.Height > resizeImage.Width)
                //    {
                //        var rectangle = new Rectangle(0, 0, Convert.ToInt32(resizeImage.Width), Convert.ToInt32(resizeImage.Height / 1.84));
                //        var cropImage = CropImage(resizeImage, rectangle);
                //        cropImage.Save(fullPath);
                //    }
                //    else
                //    {
                //        var rectangle = new Rectangle(0, 0, Convert.ToInt32(resizeImage.Width), Convert.ToInt32(resizeImage.Height * 1.84));
                //        var cropImage = CropImage(resizeImage, rectangle);
                //        cropImage.Save(fullPath);
                //    }
                //}
            }
        }

        private static Image SetHeight(Image bitmapImage, int height)
        {
            var imageHeight = bitmapImage.Height;
            var imageWidth = bitmapImage.Width;
            double ratio = Convert.ToDouble(height)/Convert.ToDouble(imageHeight);
            return ResizeImage(bitmapImage, new Size(Convert.ToInt32(imageWidth * ratio), Convert.ToInt32(imageHeight * ratio)));
        }

        private static Image SetWidth(Image bitmapImage, int width)
        {
            var imageHeight = bitmapImage.Height;
            var imageWidth = bitmapImage.Width;
            double ratio = Convert.ToDouble(width) / Convert.ToDouble(imageWidth);
            return ResizeImage(bitmapImage, new Size(Convert.ToInt32(imageWidth * ratio), Convert.ToInt32(imageHeight * ratio)));
        }

        private static Image ResizeImage(Image imgToResize, Size size)
        {
            var sourceWidth = imgToResize.Width;
            var sourceHeight = imgToResize.Height;

            //float nPercent = 0;
            //float nPercentW = 0;
            //float nPercentH = 0;

            //nPercentW = ((float)size.Width / (float)sourceWidth);
            //nPercentH = ((float)size.Height / (float)sourceHeight);

            //nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;

            var destHeight = (int) (size.Height);
            var destWidth = (int) (size.Height*sourceWidth/sourceHeight);
            
            var b = new Bitmap(destWidth, destHeight);
            var g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }

        private static Image CropImage(Image img, Rectangle cropArea)
        {
            var bmpImage = new Bitmap(img);
            var bmpCrop = bmpImage.Clone(cropArea,
            bmpImage.PixelFormat);
            return (Image)(bmpCrop);
        }
    }
}