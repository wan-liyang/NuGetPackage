
### example
```
using TryIT.SFTP;

string ip = "";
int port = 22;
string username = "";
string privateKeyFileNameAndPath = "";

var connectionInfo = SFTP.InitConenctionInfoPassword(ip, port, username, privateKeyFileNameAndPath);
var list = SFTP.ListDirectoryAndFile(connectionInfo);
```