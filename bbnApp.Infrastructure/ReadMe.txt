基础设施层（数据库访问、Dapr 封装）
--Data	EF Core 数据访问
----Migrations	EF Core 迁移文件夹
--Dapr	Dapr 封装

迁移执行

1.需要迁移的实例名称在migrations中添加
2.cmd执行指令 add-migrations-and-update.bat ApplicationDbContext，注意根据实例的对应的数据库上下文设置参数

注意在生成的时候，如果生成的迁移未见UP为空，请检查下  当前项目Data目录下配置的DbContext中反射注册中的实体类目录是否正确