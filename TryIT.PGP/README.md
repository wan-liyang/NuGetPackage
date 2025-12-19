
### Encrypt File
```
string publicKeyFile = @"C:\publicKey.asc";
string inputFile = @"C:\inputFile.docx";
//string outputFile = @"C:\inputFile.docx.pgp";

await TryIT.Pgp.PgpCrypto.EncryptFileAsync(publicKeyFile, inputFile);
```

### Decrypt File
```
string privateKeyFile = @"C:\privateKey.asc";
string passPhrase = "";
string inputFile = @"C:\inputFile.docx.pgp";
//string outputFile = @"C:\inputFile.docx";

await TryIT.Pgp.PgpCrypto.DecryptFileAsync(privateKeyFile, passPhrase, inputFile);
```