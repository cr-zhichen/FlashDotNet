using FlashDotNet.Utils;

namespace UnitTest.Utils;

[TestFixture]
public class ExpandTests
{
    #region 链式调用测试

    [Test]
    public void Do_ShouldInvokeActionOnInput()
    {
        var o = new object();

        var o2 = o.Do(_ => { });

        // Assert
        Assert.That(o2.GetType(), Is.EqualTo(o.GetType()));
    }

    #endregion

    #region md5计算测试

    [Test]
    public void ToMd5_WithValidString_ShouldReturnMd5Hash()
    {
        // Arrange
        string input = "Hello, World!";
        string expectedHash = "65a8e27d8879283831b664bd8b7f0ad4";

        // Act
        string result = input.ToMd5();

        // Assert
        Assert.That(result, Is.EqualTo(expectedHash));
    }

    [Test]
    public void ToMd5_WithValidBytes_ShouldReturnMd5Hash()
    {
        // Arrange
        byte[] input = { 72, 101, 108, 108, 111, 44, 32, 87, 111, 114, 108, 100, 33 };
        string expectedHash = "65a8e27d8879283831b664bd8b7f0ad4";

        // Act
        string result = input.ToMd5();

        // Assert
        Assert.That(result, Is.EqualTo(expectedHash));
    }

    #endregion

    #region Argon2计算测试

    [Test]
    public void ToArgon2_WithValidString_ShouldReturnArgon2Hash()
    {
        // Arrange
        string input = "Password123";
        string salt = "Salt123";
        string expectedHash = "MvrIpThJmzG7HhxDTKeM+Nyju42umAMh60YZFztU6fE=";

        // Act
        string result = input.ToArgon2(salt);

        // Assert
        Assert.That(result, Is.EqualTo(expectedHash));
    }

    #endregion

    #region 字符串转换测试

    [Test]
    public void ToInt_WithValidString_ShouldReturnInteger()
    {
        // Arrange
        string input = "123";
        int expectedValue = 123;

        // Act
        int result = input.ToInt();

        // Assert
        Assert.That(result, Is.EqualTo(expectedValue));
    }

    [Test]
    public void ToFloat_WithValidString_ShouldReturnFloat()
    {
        // Arrange
        string input = "3.14";
        float expectedValue = 3.14f;

        // Act
        float result = input.ToFloat();

        // Assert
        Assert.That(result, Is.EqualTo(expectedValue));
    }

    [Test]
    public void ToDouble_WithValidString_ShouldReturnDouble()
    {
        // Arrange
        string input = "2.71828";
        double expectedValue = 2.71828;

        // Act
        double result = input.ToDouble();

        // Assert
        Assert.That(result, Is.EqualTo(expectedValue));
    }

    #endregion

    #region 字符串格式判断测试

    [TestCase("test@example.com")]
    [TestCase("user123@gmail.com")]
    public void IsEmail_WithValidEmail_ShouldReturnTrue(string email)
    {
        // Act
        bool result = email.IsEmail();

        // Assert
        Assert.IsTrue(result);
    }

    [TestCase("13059397983")]
    [TestCase("17684383906")]
    public void IsMobile_WithValidMobileNumber_ShouldReturnTrue(string mobile)
    {
        // Act
        bool result = mobile.IsMobile();

        // Assert
        Assert.IsTrue(result);
    }

    [TestCase("210111190507283358")]
    [TestCase("65313019670926907X")]
    [TestCase("65313019670926907x")]
    public void IsIdCard_WithValidIdCardNumber_ShouldReturnTrue(string idCard)
    {
        // Act
        bool result = idCard.IsIdCard();

        // Assert
        Assert.IsTrue(result);
    }

    [TestCase("http://www.example.com")]
    [TestCase("https://www.google.com")]
    public void IsUrl_WithValidUrl_ShouldReturnTrue(string url)
    {
        // Act
        bool result = url.IsUrl();

        // Assert
        Assert.IsTrue(result);
    }

    [TestCase("192.168.0.1")]
    [TestCase("255.255.255.255")]
    public void IsIpv4_WithValidIpv4_ShouldReturnTrue(string ipv4)
    {
        // Act
        bool result = ipv4.IsIpv4();

        // Assert
        Assert.IsTrue(result);
    }

    [TestCase("2001:0db8:85a3:0000:0000:8a2e:0370:7334")]
    [TestCase("fe80::1")]
    public void IsIpv6_WithValidIpv6_ShouldReturnTrue(string ipv6)
    {
        // Act
        bool result = ipv6.IsIpv6();

        // Assert
        Assert.IsTrue(result);
    }

    [TestCase("127.0.0.1")]
    [TestCase("fe80::1")]
    public void IsIp_WithValidIp_ShouldReturnTrue(string ip)
    {
        // Act
        bool result = ip.IsIp();

        // Assert
        Assert.IsTrue(result);
    }

    [TestCase("你好")]
    [TestCase("世界")]
    public void IsChinese_WithValidChineseText_ShouldReturnTrue(string chineseText)
    {
        // Act
        bool result = chineseText.IsChinese();

        // Assert
        Assert.IsTrue(result);
    }

    [TestCase("张三")]
    [TestCase("李四")]
    public void IsChineseName_WithValidChineseName_ShouldReturnTrue(string chineseName)
    {
        // Act
        bool result = chineseName.IsChineseName();

        // Assert
        Assert.IsTrue(result);
    }

    [TestCase("abc")]
    [TestCase("xyz")]
    public void IsEnglish_WithValidEnglishText_ShouldReturnTrue(string englishText)
    {
        // Act
        bool result = englishText.IsEnglish();

        // Assert
        Assert.IsTrue(result);
    }

    [TestCase("ABC")]
    [TestCase("XYZ")]
    public void IsUpperEnglish_WithValidUppercaseEnglishText_ShouldReturnTrue(string upperEnglishText)
    {
        // Act
        bool result = upperEnglishText.IsUpperEnglish();

        // Assert
        Assert.IsTrue(result);
    }

    [TestCase("abc")]
    [TestCase("xyz")]
    public void IsLowerEnglish_WithValidLowercaseEnglishText_ShouldReturnTrue(string lowerEnglishText)
    {
        // Act
        bool result = lowerEnglishText.IsLowerEnglish();

        // Assert
        Assert.IsTrue(result);
    }

    #endregion

    #region 敏感信息处理测试

    [Test]
    public void MaskIdCard_WithValidIdCard_ShouldReturnMaskedIdCard()
    {
        // Arrange
        string idCard = "510100199001011234";
        string expectedMaskedIdCard = "510100********1234";

        // Act
        string result = idCard.MaskIdCard();

        // Assert
        Assert.That(result, Is.EqualTo(expectedMaskedIdCard));
    }

    [Test]
    public void MaskMobile_WithValidMobileNumber_ShouldReturnMaskedMobileNumber()
    {
        // Arrange
        string mobile = "18888888888";
        string expectedMaskedMobile = "188****8888";

        // Act
        string result = mobile.MaskMobile();

        // Assert
        Assert.That(result, Is.EqualTo(expectedMaskedMobile));
    }

    [Test]
    public void MaskEmail_WithValidEmail_ShouldReturnMaskedEmail()
    {
        // Arrange
        string email = "test@example.com";
        string expectedMaskedEmail = "tes****@example.com";

        // Act
        string result = email.MaskEmail();

        // Assert
        Assert.That(result, Is.EqualTo(expectedMaskedEmail));
    }

    #endregion

    #region 时间戳转换测试

    [Test]
    public void GetTotalMilliseconds_ShouldReturnMillisecondsSince1970()
    {
        // Arrange
        DateTime dt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        double expectedMilliseconds = 1672531200000;

        // Act
        double result = dt.GetTotalMilliseconds();

        // Assert
        Assert.That(result, Is.EqualTo(expectedMilliseconds));
    }

    [Test]
    public void GetTotalSeconds_ShouldReturnSecondsSince1970()
    {
        // Arrange
        DateTime dt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        double expectedSeconds = 1672531200;

        // Act
        double result = dt.GetTotalSeconds();

        // Assert
        Assert.That(result, Is.EqualTo(expectedSeconds));
    }

    [Test]
    public void ToDateTimeMilliseconds_ShouldConvertMillisecondsSince1970ToDateTime()
    {
        // Arrange
        double timestamp = 1672531200000;
        DateTime expectedDateTime = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        DateTime result = timestamp.ToDateTimeMilliseconds();

        // Assert
        Assert.That(result, Is.EqualTo(expectedDateTime));
    }

    [Test]
    public void ToDateTimeSeconds_ShouldConvertSecondsSince1970ToDateTime()
    {
        // Arrange
        double timestamp = 1672531200;
        DateTime expectedDateTime = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        DateTime result = timestamp.ToDateTimeSeconds();

        // Assert
        Assert.That(result, Is.EqualTo(expectedDateTime));
    }

    [Test]
    public void GetTimeDifference_ShouldReturnTimeDifference()
    {
        // Arrange
        DateTime dt1 = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime dt2 = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        TimeSpan expectedDifference = TimeSpan.FromHours(-12);

        // Act
        TimeSpan result = dt1.GetTimeDifference(dt2);

        // Assert
        Assert.That(result, Is.EqualTo(expectedDifference));
    }

    #endregion
}