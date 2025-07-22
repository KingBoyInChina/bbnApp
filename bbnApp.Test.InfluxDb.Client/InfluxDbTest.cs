using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Task = System.Threading.Tasks.Task;

namespace bbnApp.Test.InfluxDb.Client.Examples
{
    public static class ManagementExample
    {
        public static async Task Main()
        {
            const string url = "http://www.wushitc.cn:8086";
            const string token = "x1NAoeOsJf9ijyEXYLaqMzYA-gdbHfrYTWDcCmlcJBumPNQlM3_rIfoWkMdX7SIlZk4uX_PDCIfIBzNvQwVH6Q==";
            const string org = "lot_bbn";

            using var client = new InfluxDBClient(url, token);

            // Find ID of Organization with specified name (PermissionAPI requires ID of Organization).
            var orgId = (await client.GetOrganizationsApi().FindOrganizationsAsync(org: org)).First().Id;

            //
            // Create bucket "iot_bucket" with data retention set to 3,600 seconds
            //
            //var retention = new BucketRetentionRules(BucketRetentionRules.TypeEnum.Expire, 3600);

            //var bucket = await client.GetBucketsApi().CreateBucketAsync("write_test_data", retention, orgId);

            //
            // Create access token to "iot_bucket"
            //
            var resource = new PermissionResource(PermissionResource.TypeBuckets, "b4e01a436b8626aa", null,
                orgId);

            // Read permission
            var read = new Permission(Permission.ActionEnum.Read, resource);

            // Write permission
            var write = new Permission(Permission.ActionEnum.Write, resource);

            var authorization = await client.GetAuthorizationsApi()
                .CreateAuthorizationAsync(orgId, new List<Permission> { read, write });

            //
            // Created token that can be use for writes to "iot_bucket"
            //
            Console.WriteLine($"Authorized token to write into iot_bucket: {authorization.Token}");
        }
    }
}