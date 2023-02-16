## This service provided method to send email via SMTP

### Option 1
1. in ```Startup.cs```, call ```EmailService.Config.Configure()``` to initial Smtp Server & Port
2. in class you need send email, call ```EmailService.EmailService_Smtp.SendEmail()```

### Option 2
1. in method you need send email, call ```EmailService.Config.Configure()```
2. then call ```EmailService.EmailService_Smtp.SendEmail()```