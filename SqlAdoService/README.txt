This factory provided .NET ADO interface to perform SQL action
Has two class to use

1. SqlAdoUtility
	to use this Utility, 
	1. call SqlAdoUtility.InitConfig() in Startup.cs to initial ConnectionString & TimeoutSecond configuration
	2. use SqlAdoUtility.xxx method anywhere anytime

2. SqlAdoObject
	to use this object reference, 
	1. create new SqlAdoObject when need to use it, and pass IAdoConfig (with ConnectionString & TimeoutSecond)

