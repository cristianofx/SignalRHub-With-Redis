

Copy the file C:\Program Files\Redis\redis.windows-service.conf to redis.windows-service-1.conf and create two addicional copies, so that you end up with three files like this:

* redis.windows-service-1.conf

Create the service

```Batch
    sc create Redis1 obj= "NT AUTHORITY\NetworkService" start= auto DisplayName= "Redis1" binPath= "\"C:\Program Files\Redis\redis-server.exe\" --service-run \"C:\Program Files\Redis\redis.windows-service-1.conf\"
```

 
To remove, if necessary:

1. Remove the single instance:

```
    sc stop Redis1
    sc delete Redis1
```