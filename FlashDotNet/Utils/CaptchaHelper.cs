using System.Reflection;
using SkiaSharp;

namespace FlashDotNet.Utils;

/// <summary>
/// 验证码生成帮助类
/// </summary>
public class CaptchaHelper
{
    private readonly static Random Random = new Random();
    private readonly static SKTypeface CustomTypeface;

    /// <summary>
    /// 静态构造函数，用于初始化自定义字体
    /// </summary>
    static CaptchaHelper()
    {
        using var stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream($"{typeof(CaptchaHelper).Namespace}.Resources.Fonts.SourceHanSansSC-Bold.otf");
        CustomTypeface = stream != null
            ? SKTypeface.FromStream(stream)
            : SKTypeface.Default;

        Console.WriteLine(CustomTypeface);
    }

    /// <summary>
    /// 验证码配置选项
    /// </summary>
    public class CaptchaOptions
    {
        /// <summary>
        /// 验证码图片宽度
        /// </summary>
        public int Width { get; set; } = 130;

        /// <summary>
        /// 验证码图片高度
        /// </summary>
        public int Height { get; set; } = 50;

        /// <summary>
        /// 验证码长度
        /// </summary>
        public int Length { get; set; } = 5;

        /// <summary>
        /// 字体大小
        /// </summary>
        public int FontSize { get; set; } = 32;

        /// <summary>
        /// 干扰线数量
        /// </summary>
        public int NoiseLineCount { get; set; } = 10;

        /// <summary>
        /// 噪点数量
        /// </summary>
        public int NoisePointCount { get; set; } = 100;
    }

    /// <summary>
    /// 生成验证码结果
    /// </summary>
    public class CaptchaResult
    {
        /// <summary>
        /// 验证码文本
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 验证码图片字节数组
        /// </summary>
        public byte[] ImageBytes { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// 验证码图片的Base64编码字符串
        /// </summary>
        public string Base64Image => $"data:image/png;base64,{Convert.ToBase64String(ImageBytes)}";
    }

    /// <summary>
    /// 生成验证码
    /// </summary>
    /// <param name="options">验证码配置选项</param>
    /// <returns>验证码结果</returns>
    public static CaptchaResult Generate(CaptchaOptions? options = null)
    {
        options ??= new CaptchaOptions();
        string code = GenerateRandomCode(options.Length);
        byte[] imageBytes = GenerateCaptchaImage(code, options);

        return new CaptchaResult
        {
            Code = code,
            ImageBytes = imageBytes
        };
    }

    /// <summary>
    /// 生成随机验证码
    /// </summary>
    /// <param name="length">验证码长度</param>
    /// <returns>随机生成的验证码字符串</returns>
    private static string GenerateRandomCode(int length)
    {
        const string chars = "2345678abcdefghjkmnpqrstuvwxyzABCDEFGHJKMNPQRSTUVWXYZ";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// 生成验证码图片
    /// </summary>
    /// <param name="code">验证码文本</param>
    /// <param name="options">验证码配置选项</param>
    /// <returns>验证码图片的字节数组</returns>
    private static byte[] GenerateCaptchaImage(string code, CaptchaOptions options)
    {
        using var bitmap = new SKBitmap(new SKImageInfo(options.Width, options.Height));
        using var canvas = new SKCanvas(bitmap);

        // 填充背景
        canvas.Clear(SKColors.White);

        // 绘制背景干扰线
        DrawNoiseLines(canvas, options);

        // 绘制验证码文字
        DrawText(canvas, code, options);

        // 添加噪点
        AddNoisePoints(canvas, options);

        // 转换为字节数组
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    /// <summary>
    /// 绘制干扰线
    /// </summary>
    /// <param name="canvas">画布对象</param>
    /// <param name="options">验证码配置选项</param>
    private static void DrawNoiseLines(SKCanvas canvas, CaptchaOptions options)
    {
        using var paint = new SKPaint { IsAntialias = true };
        for (int i = 0; i < options.NoiseLineCount; i++)
        {
            paint.Color = GetRandomColor();
            paint.StrokeWidth = Random.Next(1, 3);
            canvas.DrawLine(
                Random.Next(0, options.Width), Random.Next(0, options.Height),
                Random.Next(0, options.Width), Random.Next(0, options.Height),
                paint);
        }
    }

    /// <summary>
    /// 绘制文字
    /// </summary>
    /// <param name="canvas">画布对象</param>
    /// <param name="code">验证码文本</param>
    /// <param name="options">验证码配置选项</param>
    private static void DrawText(SKCanvas canvas, string code, CaptchaOptions options)
    {
        using var paint = new SKPaint
        {
            IsAntialias = true,
            TextSize = options.FontSize,
            Typeface = CustomTypeface
        };

        float textWidth = paint.MeasureText(code);
        float startX = (options.Width - textWidth) / 2;
        float charInterval = textWidth / code.Length;

        for (int i = 0; i < code.Length; i++)
        {
            paint.Color = GetRandomColor(true);
            float x = startX + i * charInterval;
            float y = options.Height / 2 + Random.Next(-5, 5);

            canvas.Save();
            canvas.RotateDegrees(Random.Next(-90, 90), x, y);
            canvas.DrawText(code[i].ToString(), x, y + options.FontSize / 2.5f, paint);
            canvas.Restore();
        }
    }

    /// <summary>
    /// 添加噪点
    /// </summary>
    /// <param name="canvas">画布对象</param>
    /// <param name="options">验证码配置选项</param>
    private static void AddNoisePoints(SKCanvas canvas, CaptchaOptions options)
    {
        using var paint = new SKPaint
        {
            Color = SKColors.Black,
            StrokeWidth = 1
        };

        for (int i = 0; i < options.NoisePointCount; i++)
        {
            float x = Random.Next(0, options.Width);
            float y = Random.Next(0, options.Height);
            canvas.DrawPoint(x, y, paint);
        }
    }

    /// <summary>
    /// 获取随机颜色
    /// </summary>
    /// <param name="darkOnly">是否仅生成深色</param>
    /// <returns>随机颜色</returns>
    private static SKColor GetRandomColor(bool darkOnly = false)
    {
        if (darkOnly)
        {
            return new SKColor(
                (byte)Random.Next(0, 100),
                (byte)Random.Next(0, 100),
                (byte)Random.Next(0, 100),
                255);
        }

        return new SKColor(
            (byte)Random.Next(160, 256), // 调整以确保背景颜色较浅
            (byte)Random.Next(160, 256),
            (byte)Random.Next(160, 256),
            255);
    }
}
