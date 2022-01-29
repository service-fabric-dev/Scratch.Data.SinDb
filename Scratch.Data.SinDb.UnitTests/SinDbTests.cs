using System.Threading.Tasks;
using Xunit;

namespace Scratch.Data.SinDb.UnitTests
{
    public class SinDbTests
    {
        [Fact]
        public async Task CreateAsync()
        {
            var card = new Sin
            {
                Name = "AvacynAngelofHope"
            };

            var repo = new SinDb();
            var record = await repo.CreateAsync(card);
            Assert.NotNull(record);
            Assert.NotNull(record._id);
            Assert.NotNull(record.Document);
            Assert.NotNull(record.Document.Name);
            Assert.Equal(card.Name, (string) record.Document.Name);
        }
    }
}
