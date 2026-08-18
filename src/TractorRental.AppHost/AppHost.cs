var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
                      .WithManagementPlugin();

var sql = builder.AddSqlServer("sqlserver")
                 .AddDatabase("DefaultConnection");

var api = builder.AddProject<Projects.TractorRental_Api>("api")
                 .WithReference(sql)
                 .WithReference(rabbitmq)
                 .WaitFor(sql)
                 .WaitFor(rabbitmq);

var worker = builder.AddProject<Projects.TractorRental_IoTWorker>("worker")
                    .WithReference(sql)
                    .WithReference(rabbitmq)
                    .WaitFor(sql)
                    .WaitFor(rabbitmq);

builder.AddProject<Projects.TractorRental_BlazorFrontend>("frontend")
       .WithReference(api)
       .WaitFor(api);

builder.Build().Run();
