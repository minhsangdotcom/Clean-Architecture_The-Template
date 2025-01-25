To fix "exit code 78" error for elasticsearch container we will follow a few steps.

```
sudo nano /etc/sysctl.conf
```
Insert this line to sysctl.conf file

```
vm.max_map_count=262144
```

press ctrl + O to save and ctrl + X to exit, don't forget to restart your machine.

Note that it's just for linux