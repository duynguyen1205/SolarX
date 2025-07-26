using SolarX.SERVICE.Abstractions.IEmailServices;

namespace SolarX.SERVICE.Services.EmailServices;

public static class EmailExtensions
{
    public static MailContent SendAccountForStaff(string email, string fullName, string password, string role)
    {

        var body = $$"""

                     <!DOCTYPE html>
                     <html lang='en'>
                     <head>
                         <meta charset='UTF-8'>
                         <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                         <title>Your Zen Clean Account</title>
                         <style>
                             body {
                                 font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                 background-color: #f9f9f9;
                                 margin: 0;
                                 padding: 0;
                                 color: #333;
                             }
                             .container {
                                 max-width: 600px;
                                 margin: 30px auto;
                                 background-color: #fff;
                                 padding: 30px;
                                 border-radius: 10px;
                                 box-shadow: 0 4px 12px rgba(0,0,0,0.1);
                             }
                             .logo {
                                 text-align: center;
                                 margin-bottom: 20px;
                             }
                             .logo img {
                                 height: 60px;
                             }
                             h1 {
                                 color: #2F6D3E;
                                 text-align: center;
                             }
                             p {
                                 font-size: 16px;
                                 line-height: 1.6;
                             }
                             .highlight {
                                 font-weight: bold;
                                 color: #2F6D3E;
                             }
                             .password-box {
                                 background-color: #eaf6ea;
                                 border: 1px dashed #2F6D3E;
                                 padding: 15px;
                                 margin: 20px 0;
                                 text-align: center;
                                 font-size: 18px;
                                 border-radius: 8px;
                             }
                             .footer {
                                 margin-top: 30px;
                                 font-size: 13px;
                                 text-align: center;
                                 color: #999;
                             }
                             .footer a {
                                 color: #C4A232;
                                 text-decoration: none;
                             }
                         </style>
                     </head>
                     <body>
                         <div class='container'>
                             <h1>Your Zen Clean Staff Account</h1>
                             <p>Dear <span class='highlight'>{{role}}</span> <strong>{{fullName}}</strong>,</p>
                             <p>Welcome to <strong>Zen Clean Service</strong>! Your staff account has been created by the administrator.</p>
                             <p>Below are your login credentials to access the system:</p>
                             <div class='password-box'>
                                 Your account password is: <br />
                                 <span class='highlight'>{{password}}</span>
                             </div>
                             <p><strong>Important:</strong> Please change your password after logging in for better security.</p>
                             <p>If you have any questions about your account or need assistance, please contact the administrator.</p>
                             <div class='footer'>
                                 <p>&copy; 2025 Zen Clean Service. All rights reserved.</p>
                                 <p><a href='[Unsubscribe_Link]'>Unsubscribe</a> | <a href='[Privacy_Link]'>Privacy Policy</a></p>
                             </div>
                         </div>
                     </body>
                     </html>

                     """;
        return new MailContent()
        {
            Body = body,
            To = email,
            Subject = $"Your Zen Clean Staff Account - {role}",
        };
    }

    public static MailContent SendWelcomeCustomer(string email, string fullName)
    {

        var body = $$"""

                     <!DOCTYPE html>
                     <html lang='en'>
                     <head>
                         <meta charset='UTF-8'>
                         <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                         <title>Welcome to Zen Clean</title>
                         <style>
                             body {
                                 font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                 background-color: #f9f9f9;
                                 margin: 0;
                                 padding: 0;
                                 color: #333;
                             }
                             .container {
                                 max-width: 600px;
                                 margin: 30px auto;
                                 background-color: #fff;
                                 padding: 30px;
                                 border-radius: 10px;
                                 box-shadow: 0 4px 12px rgba(0,0,0,0.1);
                             }
                             .logo {
                                 text-align: center;
                                 margin-bottom: 20px;
                             }
                             .logo img {
                                 height: 60px;
                             }
                             h1 {
                                 color: #2F6D3E;
                                 text-align: center;
                             }
                             p {
                                 font-size: 16px;
                                 line-height: 1.6;
                             }
                             .highlight {
                                 font-weight: bold;
                                 color: #2F6D3E;
                             }
                             .welcome-box {
                                 background-color: #eaf6ea;
                                 border: 1px solid #2F6D3E;
                                 padding: 20px;
                                 margin: 20px 0;
                                 text-align: center;
                                 border-radius: 8px;
                             }
                             .footer {
                                 margin-top: 30px;
                                 font-size: 13px;
                                 text-align: center;
                                 color: #999;
                             }
                             .footer a {
                                 color: #C4A232;
                                 text-decoration: none;
                             }
                         </style>
                     </head>
                     <body>
                         <div class='container'>
                             <h1>Welcome to Zen Clean!</h1>
                             <p>Dear <strong>{{fullName}}</strong>,</p>
                             <p>Thank you for registering with <strong>Zen Clean Service</strong>!</p>
                             <div class='welcome-box'>
                                 <p>🎉 <strong>Registration Successful!</strong></p>
                                 <p>Your account has been created and is ready to use.</p>
                             </div>
                             <p>You can now:</p>
                             <ul>
                                 <li>Book cleaning services</li>
                                 <li>Manage your appointments</li>
                                 <li>Track your service history</li>
                                 <li>Access exclusive customer offers</li>
                             </ul>
                             <p>We're excited to provide you with the best cleaning services. Our team is committed to making your space spotless and comfortable.</p>
                             <p>If you have any questions or need assistance, feel free to contact our customer support team.</p>
                             <div class='footer'>
                                 <p>&copy; 2025 Zen Clean Service. All rights reserved.</p>
                                 <p><a href='[Unsubscribe_Link]'>Unsubscribe</a> | <a href='[Privacy_Link]'>Privacy Policy</a></p>
                             </div>
                         </div>
                     </body>
                     </html>

                     """;
        return new MailContent()
        {
            Body = body,
            To = email,
            Subject = "Welcome to Zen Clean - Registration Successful",
        };
    }

    public static MailContent SendResetPasswordEmail(string email, string firstName, string resetLink)
    {
        var body = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Reset Your Password</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 0;
            color: #333;
        }}
        
        .container {{
            max-width: 600px;
            margin: 0 auto;
            background-color: #fff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
            text-align: center;
        }}

        h1 {{
            color: #2F6D3E;
            font-size: 28px;
        }}

        p {{
            font-size: 16px;
            line-height: 1.6;
            color: #666;
        }}

        .footer {{
            text-align: center;
            margin-top: 30px;
            font-size: 12px;
            color: #aaa;
        }}

        .footer a {{
            color: #C4A232;
            text-decoration: none;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>Password Reset Request</h1>
        <p>Dear {firstName},</p>
        <p>We received a request to reset your password. Click the button below to set a new password.</p>
        <a href='{resetLink}' class='btn' 
        style=""background-color: #007bff; color: #ffffff !important; display: block; text-align: center; text-decoration: none; padding: 8px 16px; border-radius: 4px; font-size: 14px; font-weight: bold; transition: background 0.3s ease-in-out; margin: 20px auto; width: fit-content; box-shadow: 0 2px 4px rgba(0, 123, 255, 0.3);"">
          Reset Password
</a>
        
        <p>If you did not request a password reset, please ignore this email.</p>
        <div class='footer'>
            <p>&copy; 2025 Zen Clean Service. All rights reserved.</p>
            <p><a href='[Privacy_Link]'>Privacy Policy</a></p>
        </div>
    </div>
</body>
</html>
";

        return new MailContent()
        {
            Body = body,
            To = email,
            Subject = "Reset Your Password",
        };
    }

    public static MailContent SendVerificationCode(string email, string fullName, string verificationCode)
    {

        var body = $$"""

                     <!DOCTYPE html>
                     <html lang='en'>
                     <head>
                         <meta charset='UTF-8'>
                         <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                         <title>Verify Your Account</title>
                         <style>
                             body {
                                 font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                 background-color: #f9f9f9;
                                 margin: 0;
                                 padding: 0;
                                 color: #333;
                             }
                             .container {
                                 max-width: 600px;
                                 margin: 30px auto;
                                 background-color: #fff;
                                 padding: 30px;
                                 border-radius: 10px;
                                 box-shadow: 0 4px 12px rgba(0,0,0,0.1);
                             }
                             .logo {
                                 text-align: center;
                                 margin-bottom: 20px;
                             }
                             .logo img {
                                 height: 60px;
                             }
                             h1 {
                                 color: #2F6D3E;
                                 text-align: center;
                             }
                             p {
                                 font-size: 16px;
                                 line-height: 1.6;
                             }
                             .highlight {
                                 font-weight: bold;
                                 color: #2F6D3E;
                             }
                             .verification-box {
                                 background-color: #eaf6ea;
                                 border: 2px solid #2F6D3E;
                                 padding: 20px;
                                 margin: 20px 0;
                                 text-align: center;
                                 border-radius: 8px;
                             }
                             .verification-code {
                                 font-size: 36px;
                                 font-weight: bold;
                                 color: #2F6D3E;
                                 letter-spacing: 8px;
                                 margin: 10px 0;
                             }
                             .expiry-notice {
                                 background-color: #fff3cd;
                                 border: 1px solid #ffeaa7;
                                 padding: 15px;
                                 margin: 15px 0;
                                 border-radius: 5px;
                                 text-align: center;
                                 color: #856404;
                             }
                             .footer {
                                 margin-top: 30px;
                                 font-size: 13px;
                                 text-align: center;
                                 color: #999;
                             }
                             .footer a {
                                 color: #C4A232;
                                 text-decoration: none;
                             }
                         </style>
                     </head>
                     <body>
                         <div class='container'>
                             <h1>Verify Your Account</h1>
                             <p>Dear <strong>{{fullName}}</strong>,</p>
                             <p>Thank you for registering with <strong>Zen Clean Service</strong>!</p>
                             <p>To complete your registration, please use the verification code below:</p>
                             <div class='verification-box'>
                                 <p>Your verification code is:</p>
                                 <div class='verification-code'>{{verificationCode}}</div>
                             </div>
                             <div class='expiry-notice'>
                                 ⏰ <strong>This code will expire in 5 minutes</strong>
                             </div>
                             <p>Please enter this code in the verification page to activate your account.</p>
                             <p>If you didn't request this verification code, please ignore this email.</p>
                             <div class='footer'>
                                 <p>&copy; 2025 Zen Clean Service. All rights reserved.</p>
                                 <p><a href='[Unsubscribe_Link]'>Unsubscribe</a> | <a href='[Privacy_Link]'>Privacy Policy</a></p>
                             </div>
                         </div>
                     </body>
                     </html>

                     """;
        return new MailContent()
        {
            Body = body,
            To = email,
            Subject = "Verify Your Zen Clean Account",
        };
    }

    public static MailContent SendBookingConfirmation(string email, string fullName, string bookingCode,
        string serviceName, DateTimeOffset bookingDate, string bookingTime, string address, string staffName)
    {
        var formattedDate = bookingDate.ToString("dddd, MMMM dd, yyyy");

        var body = $$"""

                     <!DOCTYPE html>
                     <html lang='en'>
                     <head>
                         <meta charset='UTF-8'>
                         <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                         <title>Booking Confirmation</title>
                         <style>
                             body {
                                 font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                 background-color: #f9f9f9;
                                 margin: 0;
                                 padding: 0;
                                 color: #333;
                             }
                             .container {
                                 max-width: 600px;
                                 margin: 30px auto;
                                 background-color: #fff;
                                 padding: 30px;
                                 border-radius: 10px;
                                 box-shadow: 0 4px 12px rgba(0,0,0,0.1);
                             }
                             .logo {
                                 text-align: center;
                                 margin-bottom: 20px;
                             }
                             .logo img {
                                 height: 60px;
                             }
                             h1 {
                                 color: #2F6D3E;
                                 text-align: center;
                             }
                             p {
                                 font-size: 16px;
                                 line-height: 1.6;
                             }
                             .highlight {
                                 font-weight: bold;
                                 color: #2F6D3E;
                             }
                             .booking-details {
                                 background-color: #eaf6ea;
                                 border: 2px solid #2F6D3E;
                                 padding: 20px;
                                 margin: 20px 0;
                                 border-radius: 8px;
                             }
                             .booking-code {
                                 background-color: #f8f9fa;
                                 border: 1px dashed #2F6D3E;
                                 padding: 15px;
                                 margin: 15px 0;
                                 text-align: center;
                                 font-size: 20px;
                                 font-weight: bold;
                                 color: #2F6D3E;
                                 border-radius: 5px;
                             }
                             .detail-row {
                                 display: flex;
                                 justify-content: space-between;
                                 margin: 10px 0;
                                 padding: 8px 0;
                                 border-bottom: 1px solid #e9ecef;
                             }
                             .detail-row:last-child {
                                 border-bottom: none;
                             }
                             .detail-label {
                                 font-weight: bold;
                                 color: #2F6D3E;
                             }
                             .detail-value {
                                 color: #333;
                             }
                             .success-icon {
                                 text-align: center;
                                 font-size: 48px;
                                 color: #28a745;
                                 margin: 20px 0;
                             }
                             .contact-info {
                                 background-color: #fff3cd;
                                 border: 1px solid #ffeaa7;
                                 padding: 15px;
                                 margin: 20px 0;
                                 border-radius: 5px;
                                 text-align: center;
                             }
                             .footer {
                                 margin-top: 30px;
                                 font-size: 13px;
                                 text-align: center;
                                 color: #999;
                             }
                             .footer a {
                                 color: #C4A232;
                                 text-decoration: none;
                             }
                         </style>
                     </head>
                     <body>
                         <div class='container'>
                             <div class='success-icon'>✅</div>
                             <h1>Booking Confirmed!</h1>
                             <p>Dear <strong>{{fullName}}</strong>,</p>
                             <p>Great news! Your booking with <strong>Zen Clean Service</strong> has been confirmed by our staff member.</p>
                             
                             <div class='booking-code'>
                                 Booking Code: {{bookingCode}}
                             </div>
                             
                             <div class='booking-details'>
                                 <h3 style='color: #2F6D3E; margin-top: 0;'>📋 Booking Details</h3>
                                 <div class='detail-row'>
                                     <span class='detail-label'>Service:</span>
                                     <span class='detail-value'>{{serviceName}}</span>
                                 </div>
                                 <div class='detail-row'>
                                     <span class='detail-label'>Date:</span>
                                     <span class='detail-value'>{{formattedDate}}</span>
                                 </div>
                                 <div class='detail-row'>
                                     <span class='detail-label'>Time:</span>
                                     <span class='detail-value'>{{bookingTime}}</span>
                                 </div>
                                 <div class='detail-row'>
                                     <span class='detail-label'>Address:</span>
                                     <span class='detail-value'>{{address}}</span>
                                 </div>
                                 <div class='detail-row'>
                                     <span class='detail-label'>Confirmed by:</span>
                                     <span class='detail-value'>{{staffName}}</span>
                                 </div>
                             </div>
                             
                             <div class='contact-info'>
                                 <p><strong>📞 Our staff has called to confirm your booking</strong></p>
                                 <p>If you need to make any changes or have questions, please contact us immediately.</p>
                             </div>
                             
                             <p><strong>What's Next?</strong></p>
                             <ul>
                                 <li>Our cleaning team will arrive at your location on the scheduled date and time</li>
                                 <li>Please ensure someone is available to provide access to your property</li>
                                 <li>Have your booking code ready for verification</li>
                                 <li>Any special instructions should be communicated to our team upon arrival</li>
                             </ul>
                             
                             <p>Thank you for choosing Zen Clean Service. We look forward to providing you with excellent cleaning services!</p>
                             
                             <div class='footer'>
                                 <p>&copy; 2025 Zen Clean Service. All rights reserved.</p>
                                 <p><a href='[Unsubscribe_Link]'>Unsubscribe</a> | <a href='[Privacy_Link]'>Privacy Policy</a></p>
                             </div>
                         </div>
                     </body>
                     </html>

                     """;
        return new MailContent()
        {
            Body = body,
            To = email,
            Subject = $"Booking Confirmed - {bookingCode} | {serviceName}",
        };
    }

    public static MailContent SendNewConsultingRequestToAdmin(string email, string adminName, string requestName, string requestPhone,
        string area)
    {
        var body = $$"""
                     <!DOCTYPE html>
                     <html lang='vi'>
                     <head>
                         <meta charset='UTF-8'>
                         <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                         <title>Thông báo Yêu cầu Tư vấn mới</title>
                         <style>
                             body {
                                 font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                 background-color: #f9f9f9;
                                 margin: 0;
                                 padding: 0;
                                 color: #333;
                             }
                             .container {
                                 max-width: 600px;
                                 margin: 30px auto;
                                 background-color: #fff;
                                 padding: 30px;
                                 border-radius: 10px;
                                 box-shadow: 0 4px 12px rgba(0,0,0,0.1);
                             }
                             h1 {
                                 color: #2F6D3E;
                                 text-align: center;
                             }
                             .notification-box {
                                 background-color: #fffbe6;
                                 border: 2px solid #C4A232;
                                 padding: 20px;
                                 margin: 20px 0;
                                 text-align: center;
                                 border-radius: 8px;
                             }
                             .highlight {
                                 font-weight: bold;
                                 color: #C4A232;
                             }
                             .footer {
                                 margin-top: 30px;
                                 font-size: 13px;
                                 text-align: center;
                                 color: #999;
                             }
                         </style>
                     </head>
                     <body>
                         <div class='container'>
                             <h1>📞 Thông báo Yêu cầu Tư vấn mới</h1>
                             <p>Kính gửi <strong>{{adminName}}</strong>,</p>
                             <p>Hệ thống vừa nhận được một <span class='highlight'>yêu cầu tư vấn mới</span> từ khách hàng tại chi nhánh của bạn:</p>
                             
                             <div class='notification-box'>
                                 <p><strong>📝 Thông tin yêu cầu tư vấn</strong></p>
                                 <p><span class='highlight'>Khách hàng:</span> {{requestName}}</p>
                                 <p><span class='highlight'>Số điện thoại:</span> {{requestPhone}}</p>
                                 <p><span class='highlight'>Khu vực quan tâm:</span> {{area}}</p>
                             </div>
                             
                             <p>Vui lòng đăng nhập hệ thống để xem chi tiết và liên hệ khách hàng sớm nhất có thể.</p>
                             
                             <div class='footer'>
                                 <p>&copy; 2025 SolarX. All rights reserved.</p>
                                 <p>Đây là email thông báo tự động, vui lòng không trả lời email này.</p>
                             </div>
                         </div>
                     </body>
                     </html>
                     """;

        return new MailContent()
        {
            Body = body,
            To = email,
            Subject = "Yêu cầu tư vấn mới từ khách hàng",
        };
    }


}