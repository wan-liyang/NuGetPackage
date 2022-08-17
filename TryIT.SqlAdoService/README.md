## Provided .NET ADO method to perform SQL action

### How to use ```SqlAdoStatic```

1. call ```SqlAdoUtility.InitConfig()``` in ```Startup.cs``` to initial ConnectionString & TimeoutSecond configuration
2. use ```SqlAdoUtility.xxx``` method anywhere anytime

### How to use ```SqlAdoObject```
1. create new ```SqlAdoObject``` when need to use it, and pass ```IAdoConfig``` (with ConnectionString & TimeoutSecond)

