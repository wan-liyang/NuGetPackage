This service provided method to send email via SMTP
Has two class to use

1. EmailService_Smtp
	to use this service, 
	1. in Startup.cs, call EmailService.Config.Configure() to initial Smtp Server & Port
	2. FromAddress is optional, if initial at here, then not need provide same address
	3. in class you need send email, call EmailService.EmailService_Smtp.SendEmail()
