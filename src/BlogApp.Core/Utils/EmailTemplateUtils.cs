namespace BlogApp.Core.Utils;

public abstract class EmailTemplateUtils
{
    public static string GetVerificationEmailTemplate(string username, string verificationLink)
    {
        return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .button {{
                        display: inline-block;
                        padding: 10px 20px;
                        background-color: #4CAF50;
                        color: white;
                        text-decoration: none;
                        border-radius: 5px;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <h2>Welcome, {username}!</h2>
                    <p>Please verify your email address to complete your registration.</p>
                    <a href='{verificationLink}' class='button'>Verify Email</a>
                    <p>Or copy and paste this link into your browser:</p>
                    <p><small>{verificationLink}</small></p>
                    <p>This link will expire in 24 hours.</p>
                </div>
            </body>
            </html>";
    }
}