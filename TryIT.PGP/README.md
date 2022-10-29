
### Encrypt File
```
string publicKeyFile = @"C:\publicKey.asc";
string inputFile = @"C:\inputFile.docx";
//string outputFile = @"C:\inputFile.docx.pgp";

TryIT.PGP.PgpEncryption.EncryptFile(publicKeyFile, inputFile);
```

### Decrypt File
```
string privateKeyFile = @"C:\privateKey.asc";
string passPhrase = "";
string inputFile = @"C:\inputFile.docx.pgp";
//string outputFile = @"C:\inputFile.docx";

TryIT.PGP.PgpEncryption.DecryptFile(privateKeyFile, passPhrase, inputFile);
```