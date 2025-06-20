using FluentMigrator;

namespace BlogDoFt.DeveloperToolbox.Api.Database.Migrations;

[Migration(version: 202506141518, description: "Create Secrets table.")]
public class M202506141518CreateEncryptedFilesTable : Migration
{
    public override void Up()
    {
        Create.Table("encrypted_files")
            .WithColumn("id").AsInt32().PrimaryKey().Identity()
            .WithColumn("navigation_id").AsGuid().NotNullable().Indexed("idx_sh_encrypted_files_navigationid")
            .WithColumn("file_name").AsString().NotNullable().Unique("idx_uk_encrypted_files_file_name")
            .WithColumn("content").AsCustom("BYTEA").NotNullable()
            .WithColumn("iv").AsCustom("BYTEA").NotNullable()
            .WithColumn("created_at").AsDateTime().WithDefault(SystemMethods.CurrentDateTime);
    }

    public override void Down()
    {
        Delete.Table("encrypted_files");
    }
}
