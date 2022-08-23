### This service provided method to send email via SMTP

1. in ```Startup.cs```, call ```EmailService.Config.Configure()``` to initial Smtp Server & Port
2. FromAddress is optional, if initial at here, then not need provide same
3. in class you need send email, call ```EmailService.EmailService_Smtp.SendEmail()```
