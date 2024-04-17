using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace API.Seeds;

public class SystemLanguageSeed : IEntityTypeConfiguration<SystemLanguage>
{
    public void Configure(EntityTypeBuilder<SystemLanguage> builder)
    {
        builder.HasData(
            new SystemLanguage
            {
                LanguageCode = "en_US",
                LanguageName = "English",
                UrlImage = "en.png"
            },
            new SystemLanguage
            {
                LanguageCode = "vi_VN",
                LanguageName = "Vietnamese",
                UrlImage = "vn.png"
            },
            new SystemLanguage
            {
                LanguageCode = "zh_TW",
                LanguageName = "繁體中文",
                UrlImage = "zh.png"
            }
        );
    }
}
