bbnApp.MQTT.DataReciveClient
--这是个独立部署的数据采集应用将数据写入RabbitMQ队列，然后队列所在的应用再将数据写入influxdb，这个应用只负责数据采集和写入RabbitMQ队列！！！需要与MQTT服务端配合使用！！！最好放在一起，那样不用占用带宽，直接通过本地网络进行数据传输！！！