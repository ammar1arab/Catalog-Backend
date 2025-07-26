using Backend.DbContexts;
using Backend.DTOs.Items;
using Backend.Helpers;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Implementations
{
    public class ItemServices : IItemServices
    {
        private readonly CatalogContext _context;
        private readonly string _imagesFolderPath;

        public ItemServices(CatalogContext context, IWebHostEnvironment env)
        {
            _context = context;
            _imagesFolderPath = Path.Combine(env.WebRootPath, "UploadedImages");

            if (!Directory.Exists(_imagesFolderPath))
                Directory.CreateDirectory(_imagesFolderPath);
        }

        public async Task<List<GetItemsDto>> GlobalSearchItemsAsync(string term, HttpRequest request, int page = 1, int pageSize = 30)
        {
            var lowered = term.Trim().ToLower();

            var query = _context.Items
                .Include(i => i.Status)
                .Where(i =>
                    i.StatusId != "1" &&
                    (EF.Functions.Like(i.ItemNo.ToLower(), $"%{lowered}%") ||
                     EF.Functions.Like(i.Name.ToLower(), $"%{lowered}%")));

            var pagedItems = await query
                .OrderBy(i => i.ItemNo)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return pagedItems.Select(i => MapToDto(i, request)).ToList();
        }



        public async Task<List<GetItemsDto>> GetItemsWithPaginationAsync( string groupId, string subOneId, string? subTwoId, string? subThreeId, HttpRequest request, int page = 1, int pageSize = 30)
        {
            var baseQuery = _context.Items
                .Include(i => i.Status)
                .Where(i =>
                    i.GroupId == groupId &&
                    i.SubOneId == subOneId &&
                    i.StatusId != "1");

            if (!string.IsNullOrWhiteSpace(subThreeId) && subThreeId != "0")
            {
                baseQuery = baseQuery.Where(i => i.SubThreeId == subThreeId);
            }
            else if (!string.IsNullOrWhiteSpace(subTwoId))
            {
                baseQuery = baseQuery.Where(i =>
                    i.SubTwoId == subTwoId &&
                    (i.SubThreeId == null || i.SubThreeId.Trim() == "" || i.SubThreeId.Trim() == "0"));
            }

            var pagedItems = await baseQuery
                .OrderBy(i => i.ItemNo)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return pagedItems.Select(i => MapToDto(i, request)).ToList();
        }

        public async Task<List<GetItemsDto>> SearchItemsAsync(string term,string groupId,string subOneId,string? subTwoId,string? subThreeId,HttpRequest request,int page = 1,int pageSize = 30)
        {
            var lowered = term.Trim().ToLower();

            var query = _context.Items
                .Include(i => i.Status)
                .Where(i =>
                    i.GroupId == groupId &&
                    i.SubOneId == subOneId &&
                    i.StatusId != "1" &&
                    (EF.Functions.Like(i.ItemNo.ToLower(), $"%{lowered}%") ||
                     EF.Functions.Like(i.Name.ToLower(), $"%{lowered}%")));

            if (!string.IsNullOrWhiteSpace(subThreeId))
            {
                query = query.Where(i => i.SubThreeId == subThreeId);
            }
            else if (!string.IsNullOrWhiteSpace(subTwoId))
            {
                query = query.Where(i =>
                    i.SubTwoId == subTwoId &&
                    (i.SubThreeId == null || i.SubThreeId == "0" || i.SubThreeId == ""));
            }

            var pagedItems = await query
                .OrderBy(i => i.ItemNo)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return pagedItems.Select(i => MapToDto(i, request)).ToList();
        }


        public async Task<List<GetItemsDto>> GetItemsByStatusAsync(string statusId, HttpRequest request, int page = 1, int pageSize = 30)
        {
            var items = await _context.Items
                .Include(i => i.Status)
                .AsNoTracking()
                .Where(i => i.StatusId == statusId)
                .OrderBy(i => i.ItemNo)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return items.Select(i => MapToDto(i, request)).ToList();
        }

        public async Task<List<ItemStatusDto>> GetAllItemStatusesAsync(HttpRequest request)
        {
            return await _context.LookupItems
                .Where(x => x.LookupTypeId == "1")
                .Select(x => new ItemStatusDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    IconUrl = UrlHelper.GetStatusIconUrl(x.IconPath, request)
                })
                .ToListAsync();
        }

        public async Task<GetItemDto?> GetItemByItemNoAsync(string itemNo)
        {
            var item = await _context.Items.FindAsync(itemNo);
            if (item == null) return null;

            return new GetItemDto
            {
                ItemNo = item.ItemNo,
                Name = item.Name,
                Description = item.Description,
                Images = item.Images
            };
        }

        public async Task<string?> GetImageByItemNoAndImageIdAsync(string itemNo, string imageId)
        {
            var item = await _context.Items.AsNoTracking().FirstOrDefaultAsync(x => x.ItemNo == itemNo);
            return item?.Images?.FirstOrDefault(img => img == imageId);
        }

        public async Task AddImagesToItemAsync(string itemNo, List<IFormFile>? images)
        {
            var item = await _context.Items.FindAsync(itemNo)
                ?? throw new KeyNotFoundException("Item not found.");

            if (images?.Any() == true)
            {
                var newImageNames = new List<string>();
                foreach (var image in images)
                {
                    var fileName = FileHelper.GenerateImageFileName(image);
                    var savePath = FileHelper.GetImageSavePath(_imagesFolderPath, fileName);
                    await ImageHelper.CompressAndSaveAsync(image, savePath);
                    newImageNames.Add(fileName);
                }

                item.Images = (item.Images ?? new List<string>()).Concat(newImageNames).ToList();
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<string>> GetItemImagesAsync(string itemNo, HttpRequest request)
        {
            var item = await _context.Items.AsNoTracking().FirstOrDefaultAsync(i => i.ItemNo == itemNo);
            if (item == null) throw new KeyNotFoundException("Item not found.");

            return (item.Images ?? new List<string>()).Select(img => UrlHelper.GetItemImageUrl(img, request)).ToList();
        }

        public async Task DeleteItemImagesAsync(string itemNo, List<string> imagesToDelete)
        {
            var item = await _context.Items.FindAsync(itemNo)
                ?? throw new KeyNotFoundException("Item not found.");

            if (item.Images == null || !item.Images.Any()) return;

            foreach (var imageName in imagesToDelete)
            {
                var path = Path.Combine(_imagesFolderPath, imageName);
                if (File.Exists(path)) File.Delete(path);
            }

            item.Images = item.Images.Where(img => !imagesToDelete.Contains(img)).ToList();
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateItemStatusAsync(string itemNo, string statusId)
        {
            var item = await _context.Items.FindAsync(itemNo);
            if (item == null) return false;

            item.StatusId = statusId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateItemDescriptionAsync(string itemNo, string? description)
        {
            var item = await _context.Items.FindAsync(itemNo);
            if (item == null) return false;

            item.Description = description;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ItemStatusDto?> GetItemStatusAsync(string itemNo, HttpRequest request)
        {
            var item = await _context.Items.AsNoTracking()
                .Include(i => i.Status)
                .FirstOrDefaultAsync(i => i.ItemNo == itemNo);

            if (item?.Status == null) return null;

            return new ItemStatusDto
            {
                Id = item.Status.Id,
                Name = item.Status.Name,
                Code = item.Status.Code,
                IconUrl = UrlHelper.GetStatusIconUrl(item.Status.IconPath, request)
            };
        }

        private GetItemsDto MapToDto(Entities.Item i, HttpRequest request)
        {
            return new GetItemsDto
            {
                ItemNo = i.ItemNo,
                Name = i.Name,
                FirstImage = i.Images?.FirstOrDefault() ?? "no-image.png",
                Status = i.Status == null ? null : new ItemStatusDto
                {
                    Id = i.Status.Id,
                    Name = i.Status.Name,
                    Code = i.Status.Code,
                    IconUrl = UrlHelper.GetStatusIconUrl(i.Status.IconPath, request)
                }
            };
        }
    }
}
