using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Scratch.Core.Extensions;
using Scratch.Data.LiteDb.Models;

namespace Scratch.Data.SinDb
{
    public class SinDb
    {
        private static readonly List<char> InvalidChars = Path.GetInvalidFileNameChars().ToList();

        public async Task<LiteRecord<Sin>> CreateAsync(Sin sin)
        {
            sin.Guard(nameof(sin));
            sin.Name.GuardNullOrWhiteSpace(nameof(sin.Name));

            var validNameParts = GetValidNameParts(sin);
            var validName = GetValidName(validNameParts);
            var path = GetValidPath(validNameParts, validName);

            var record = new LiteRecord<Sin>
            {
                Document = sin
            };

            using var db = new LiteDatabase(path);

            var collection = db.GetCollection<LiteRecord<Sin>>(validName);
            var idBson = collection.Insert(record);
            record._id = idBson.AsObjectId;

            return record;
        }

        private List<string> GetValidNameParts(Sin card)
        {
            var nameChars = card.Name.ToCharArray();
            var validChars = nameChars.Where(c => !InvalidChars.Contains(c));
            var validNameParts = validChars.Select(c => c.ToString()).ToList();
            return validNameParts.ToList();
        }

        private string GetValidName(IEnumerable<string> validNameParts)
        {
            var validName = validNameParts.Aggregate(string.Concat);
            return validName;
        }

        private string GetValidPath(IEnumerable<string> validNameParts, string validName)
        {
            var validDirectory = validNameParts.Aggregate(Path.Combine);
            var path = $"{Path.Combine(validDirectory, validName)}.db".EnsureDirectoryExists();
            return path.FullName;
        }
    }
}