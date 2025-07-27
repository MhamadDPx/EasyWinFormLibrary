using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace EasyWinFormLibrary.WinAppNeeds
{
    /// <summary>
    /// Provides comprehensive image processing utilities for conversion, compression, resizing, and manipulation.
    /// Optimized for .NET Framework 4.8 with proper resource management and error handling.
    /// </summary>
    public static class ImageUtils
    {
        #region Compression and Conversion

        /// <summary>
        /// Converts an image to a compressed JPEG format with specified quality and automatic EXIF orientation correction.
        /// </summary>
        /// <param name="img">The source image to compress</param>
        /// <param name="compressionQuality">JPEG quality level (0-100, where 100 is highest quality)</param>
        /// <returns>Compressed JPEG image as byte array</returns>
        /// <exception cref="ArgumentNullException">Thrown when img is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when compressionQuality is not between 0-100</exception>
        public static byte[] ConvertToCompressedJpeg(Image img, int compressionQuality)
        {
            if (img == null)
                throw new ArgumentNullException(nameof(img), "Image cannot be null");

            if (compressionQuality < 0 || compressionQuality > 100)
                throw new ArgumentOutOfRangeException(nameof(compressionQuality), "Quality must be between 0 and 100");

            try
            {
                using (var compressedStream = new MemoryStream())
                {
                    // Set compression quality for the JPEG encoder
                    using (var encoderParams = new EncoderParameters(1))
                    {
                        encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, (long)compressionQuality);

                        // Get the JPEG codec
                        var jpegCodec = GetEncoderInfo(ImageFormat.Jpeg);
                        if (jpegCodec == null)
                            throw new InvalidOperationException("JPEG encoder not found");

                        // Create oriented image with proper resource management
                        using (var orientedImage = CreateOrientedImage(img))
                        {
                            // Save the oriented image to the compressed stream
                            orientedImage.Save(compressedStream, jpegCodec, encoderParams);
                        }

                        return compressedStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to compress image: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Converts an image to a byte array in the specified format with automatic EXIF orientation correction.
        /// </summary>
        /// <param name="img">The source image to convert</param>
        /// <param name="format">The target image format (JPEG, PNG, BMP, etc.)</param>
        /// <returns>Image as byte array in the specified format</returns>
        /// <exception cref="ArgumentNullException">Thrown when img or format is null</exception>
        public static byte[] ConvertImageToByte(Image img, ImageFormat format)
        {
            if (img == null)
                throw new ArgumentNullException(nameof(img), "Image cannot be null");
            if (format == null)
                throw new ArgumentNullException(nameof(format), "Format cannot be null");

            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var orientedImage = CreateOrientedImage(img))
                    {
                        orientedImage.Save(ms, format);
                    }
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert image to byte array: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Converts a byte array to an Image object.
        /// </summary>
        /// <param name="data">The byte array containing image data</param>
        /// <returns>Image object created from the byte array</returns>
        /// <exception cref="ArgumentNullException">Thrown when data is null</exception>
        /// <exception cref="ArgumentException">Thrown when data is empty or invalid</exception>
        public static Image ConvertByteToImage(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data), "Data cannot be null");
            if (data.Length == 0)
                throw new ArgumentException("Data cannot be empty", nameof(data));

            try
            {
                using (var ms = new MemoryStream(data))
                {
                    // Create a copy to avoid issues with stream disposal
                    return new Bitmap(Image.FromStream(ms));
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert byte array to image: {ex.Message}", ex);
            }
        }

        #endregion

        #region Resizing and Scaling

        /// <summary>
        /// Resizes an image to the specified dimensions while maintaining aspect ratio.
        /// </summary>
        /// <param name="originalImage">The source image to resize</param>
        /// <param name="maxWidth">Maximum width of the resized image</param>
        /// <param name="maxHeight">Maximum height of the resized image</param>
        /// <param name="maintainAspectRatio">Whether to maintain the original aspect ratio (default: true)</param>
        /// <returns>Resized image</returns>
        public static Image ResizeImage(Image originalImage, int maxWidth, int maxHeight, bool maintainAspectRatio = true)
        {
            if (originalImage == null)
                throw new ArgumentNullException(nameof(originalImage));
            if (maxWidth <= 0 || maxHeight <= 0)
                throw new ArgumentException("Width and height must be positive values");

            int newWidth, newHeight;

            if (maintainAspectRatio)
            {
                var ratioX = (double)maxWidth / originalImage.Width;
                var ratioY = (double)maxHeight / originalImage.Height;
                var ratio = Math.Min(ratioX, ratioY);

                newWidth = (int)(originalImage.Width * ratio);
                newHeight = (int)(originalImage.Height * ratio);
            }
            else
            {
                newWidth = maxWidth;
                newHeight = maxHeight;
            }

            var resizedImage = new Bitmap(newWidth, newHeight);
            using (var graphics = Graphics.FromImage(resizedImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }

            return resizedImage;
        }

        /// <summary>
        /// Creates a thumbnail of the specified image with high quality scaling.
        /// </summary>
        /// <param name="originalImage">The source image</param>
        /// <param name="thumbnailSize">Size of the thumbnail (both width and height)</param>
        /// <returns>Square thumbnail image</returns>
        public static Image CreateThumbnail(Image originalImage, int thumbnailSize)
        {
            if (originalImage == null)
                throw new ArgumentNullException(nameof(originalImage));
            if (thumbnailSize <= 0)
                throw new ArgumentException("Thumbnail size must be positive", nameof(thumbnailSize));

            return ResizeImage(originalImage, thumbnailSize, thumbnailSize, true);
        }

        #endregion

        #region Image Manipulation

        /// <summary>
        /// Rotates an image by the specified angle.
        /// </summary>
        /// <param name="image">The source image to rotate</param>
        /// <param name="angle">Rotation angle in degrees (90, 180, 270, or custom)</param>
        /// <returns>Rotated image</returns>
        public static Image RotateImage(Image image, float angle)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            var rotatedImage = new Bitmap(image.Width, image.Height);
            using (var graphics = Graphics.FromImage(rotatedImage))
            {
                graphics.TranslateTransform(image.Width / 2f, image.Height / 2f);
                graphics.RotateTransform(angle);
                graphics.TranslateTransform(-image.Width / 2f, -image.Height / 2f);
                graphics.DrawImage(image, new Point(0, 0));
            }
            return rotatedImage;
        }

        /// <summary>
        /// Crops an image to the specified rectangle.
        /// </summary>
        /// <param name="image">The source image to crop</param>
        /// <param name="cropArea">The rectangle defining the crop area</param>
        /// <returns>Cropped image</returns>
        public static Image CropImage(Image image, Rectangle cropArea)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));
            if (cropArea.Width <= 0 || cropArea.Height <= 0)
                throw new ArgumentException("Crop area must have positive dimensions");

            var croppedImage = new Bitmap(cropArea.Width, cropArea.Height);
            using (var graphics = Graphics.FromImage(croppedImage))
            {
                graphics.DrawImage(image, new Rectangle(0, 0, cropArea.Width, cropArea.Height), cropArea, GraphicsUnit.Pixel);
            }
            return croppedImage;
        }

        /// <summary>
        /// Converts an image to grayscale.
        /// </summary>
        /// <param name="image">The source image to convert</param>
        /// <returns>Grayscale version of the image</returns>
        public static Image ConvertToGrayscale(Image image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            var grayscaleImage = new Bitmap(image.Width, image.Height);
            using (var graphics = Graphics.FromImage(grayscaleImage))
            {
                // Create grayscale color matrix
                var colorMatrix = new ColorMatrix(new float[][]
                {
                    new float[] {0.299f, 0.299f, 0.299f, 0, 0},
                    new float[] {0.587f, 0.587f, 0.587f, 0, 0},
                    new float[] {0.114f, 0.114f, 0.114f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                });

                using (var attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(colorMatrix);
                    graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                        0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            return grayscaleImage;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets the appropriate image encoder for the specified format.
        /// </summary>
        /// <param name="format">The image format to find encoder for</param>
        /// <returns>ImageCodecInfo for the specified format, or null if not found</returns>
        public static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            if (format == null)
                throw new ArgumentNullException(nameof(format));

            return ImageCodecInfo.GetImageEncoders()
                .FirstOrDefault(codec => codec.FormatID == format.Guid);
        }

        /// <summary>
        /// Creates a properly oriented image based on EXIF orientation data.
        /// </summary>
        /// <param name="originalImage">The source image that may need orientation correction</param>
        /// <returns>Image with correct orientation applied</returns>
        public static Bitmap CreateOrientedImage(Image originalImage)
        {
            if (originalImage == null)
                throw new ArgumentNullException(nameof(originalImage));

            var orientedImage = new Bitmap(originalImage.Width, originalImage.Height);
            using (var graphics = Graphics.FromImage(orientedImage))
            {
                graphics.Clear(Color.White);
                graphics.DrawImage(originalImage, new Rectangle(0, 0, originalImage.Width, originalImage.Height));
            }

            // Check if we need to rotate the image based on EXIF data
            if (originalImage.PropertyIdList.Contains(0x0112)) // 0x0112 is the EXIF orientation tag
            {
                var orientationProperty = originalImage.GetPropertyItem(0x0112);
                if (orientationProperty?.Value?.Length > 0)
                {
                    var orientation = (int)orientationProperty.Value[0];
                    switch (orientation)
                    {
                        case 6: // Rotate 90 degrees clockwise
                            orientedImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                        case 3: // Rotate 180 degrees
                            orientedImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case 8: // Rotate 270 degrees clockwise (90 degrees counter-clockwise)
                            orientedImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                    }
                }
            }

            return orientedImage;
        }

        /// <summary>
        /// Validates if the provided byte array contains valid image data.
        /// </summary>
        /// <param name="data">Byte array to validate</param>
        /// <returns>True if data represents a valid image, false otherwise</returns>
        public static bool IsValidImageData(byte[] data)
        {
            if (data == null || data.Length == 0)
                return false;

            try
            {
                using (var ms = new MemoryStream(data))
                using (var img = Image.FromStream(ms))
                {
                    return img.Width > 0 && img.Height > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the image format from a byte array by examining the file header.
        /// </summary>
        /// <param name="data">Image data as byte array</param>
        /// <returns>Detected ImageFormat, or null if format cannot be determined</returns>
        public static ImageFormat GetImageFormat(byte[] data)
        {
            if (data == null || data.Length < 4)
                return null;

            // Check for common image format signatures
            if (data[0] == 0xFF && data[1] == 0xD8) // JPEG
                return ImageFormat.Jpeg;
            if (data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47) // PNG
                return ImageFormat.Png;
            if (data[0] == 0x42 && data[1] == 0x4D) // BMP
                return ImageFormat.Bmp;
            if (data[0] == 0x47 && data[1] == 0x49 && data[2] == 0x46) // GIF
                return ImageFormat.Gif;

            return null;
        }

        /// <summary>
        /// Calculates the file size reduction percentage after compression.
        /// </summary>
        /// <param name="originalSize">Original file size in bytes</param>
        /// <param name="compressedSize">Compressed file size in bytes</param>
        /// <returns>Compression ratio as percentage (0-100)</returns>
        public static double CalculateCompressionRatio(long originalSize, long compressedSize)
        {
            if (originalSize <= 0)
                throw new ArgumentException("Original size must be positive", nameof(originalSize));
            if (compressedSize < 0)
                throw new ArgumentException("Compressed size cannot be negative", nameof(compressedSize));

            return Math.Round(((double)(originalSize - compressedSize) / originalSize) * 100, 2);
        }

        #endregion
    }
}