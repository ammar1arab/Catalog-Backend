using Azure.Core;
using Backend.DbContexts;
using Backend.DTOs.Categories;
using Backend.Helpers;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Implementations
{
    public class CategoriesServices : ICategoriesServices
    {
        private readonly CatalogContext _context;
        private readonly string _categoryImagesPath;

        public CategoriesServices(CatalogContext context, IWebHostEnvironment env)
        {
            _context = context;
            _categoryImagesPath = Path.Combine(env.WebRootPath, "CategoriesImages");

            if (!Directory.Exists(_categoryImagesPath))
                Directory.CreateDirectory(_categoryImagesPath);
        }

        // ✅ Get Groups
        public async Task<List<GroupDto>> GetAllGroups(HttpRequest request)
        {
            var groups = await _context.Groups.AsNoTracking().ToListAsync();
            return groups.Select(g => new GroupDto
            {
                Id = g.Id,
                Name = g.Name,
                ImageUrl = UrlHelper.GetCategoryImageUrl(g.Image, request)
            }).ToList();
        }

        // ✅ Get SubOnes
        public async Task<List<SubOneDto>> GetAllSubOnesByGroupId(string groupId, HttpRequest request)
        {
            if (!await _context.Groups.AnyAsync(g => g.Id == groupId))
                throw new KeyNotFoundException("Group not found");

            var subOnes = await _context.SubOnes
                .Where(s => s.GroupId == groupId)
                .AsNoTracking()
                .ToListAsync();

            return subOnes.Select(s => new SubOneDto
            {
                Id = s.Id,
                Name = s.Name,
                ImageUrl = UrlHelper.GetCategoryImageUrl(s.Image, request)
            }).ToList();
        }

        // ✅ Get SubTwos
        public async Task<List<SubTwoDto>> GetAllSubTwosByGroupAndSubOne(string groupId, string subOneId, HttpRequest request)
        {
            var exists = await _context.SubOnes
                .AnyAsync(s => s.Id == subOneId && s.GroupId == groupId);

            if (!exists)
                throw new KeyNotFoundException("SubOne not found under the given Group");

            var subTwos = await _context.SubTwos
                .Where(s => s.SubOneId == subOneId && s.GroupId == groupId)
                .AsNoTracking()
                .ToListAsync();

            return subTwos.Select(st => new SubTwoDto
            {
                Id = st.Id,
                Name = st.Name,
                ImageUrl = UrlHelper.GetCategoryImageUrl(st.Image, request)
            }).ToList();
        }

        // ✅ Get SubThrees
        public async Task<List<SubThreeDto>> GetAllSubThreesByFullPath(string groupId, string subOneId, string subTwoId, HttpRequest request)
        {
            var exists = await _context.SubTwos
                .AnyAsync(s => s.Id == subTwoId && s.SubOneId == subOneId && s.GroupId == groupId);

            if (!exists)
                throw new KeyNotFoundException("SubTwo not found under the given SubOne and Group");

            var subThrees = await _context.SubThrees
                .Where(s => s.SubTwoId == subTwoId && s.SubOneId == subOneId && s.GroupId == groupId)
                .AsNoTracking()
                .ToListAsync();

            return subThrees.Select(st => new SubThreeDto
            {
                Id = st.Id,
                Name = st.Name,
                ImageUrl = UrlHelper.GetCategoryImageUrl(st.Image, request)
            }).ToList();
        }

        // ✅ Upload Images
        public async Task UploadGroupImageAsync(string id, IFormFile file)
        {
            var group = await _context.Groups.FindAsync(id)
                ?? throw new KeyNotFoundException("Group doesn't exist");

            if (!string.IsNullOrEmpty(group.Image) && group.Image != "no-image.png")
                DeleteCategoryImage(group.Image);

            group.Image = await UploadCategoryImageAsync(file);
            await _context.SaveChangesAsync();
        }

        public async Task UploadSubOneImageAsync(string id, IFormFile file)
        {
            var subOne = await _context.SubOnes.FindAsync(id)
                ?? throw new KeyNotFoundException("SubOne doesn't exist");

            if (!string.IsNullOrEmpty(subOne.Image)  && subOne.Image != "no-image.png")
                DeleteCategoryImage(subOne.Image);

            subOne.Image = await UploadCategoryImageAsync(file);
            await _context.SaveChangesAsync();
        }

        public async Task UploadSubTwoImageAsync(string id, IFormFile file)
        {
            var subTwo = await _context.SubTwos.FindAsync(id)
                ?? throw new KeyNotFoundException("SubTwo doesn't exist");

            if (!string.IsNullOrEmpty(subTwo.Image)  && subTwo.Image != "no-image.png")
                DeleteCategoryImage(subTwo.Image);

            subTwo.Image = await UploadCategoryImageAsync(file);
            await _context.SaveChangesAsync();
        }

        public async Task UploadSubThreeImageAsync(string id, IFormFile file)
        {
            var subThree = await _context.SubThrees.FindAsync(id)
                ?? throw new KeyNotFoundException("SubThree doesn't exist");

            if (!string.IsNullOrEmpty(subThree.Image)  && subThree.Image != "no-image.png")
                DeleteCategoryImage(subThree.Image);

            subThree.Image = await UploadCategoryImageAsync(file);
            await _context.SaveChangesAsync();
        }

        // ✅ Delete Images
        public async Task<string> DeleteGroupImageAsync(string id, string imageUrl, HttpRequest request)
        {
            var group = await _context.Groups.FindAsync(id)
                ?? throw new KeyNotFoundException("Group doesn't exist");

            if (string.IsNullOrWhiteSpace(group.Image) || group.Image != imageUrl)
                throw new InvalidOperationException("Image not found or doesn't match");

            DeleteCategoryImage(group.Image);

            group.Image = "no-image.png";
            await _context.SaveChangesAsync();

            var baseUrl = $"{request.Scheme}://{request.Host}";
            return UrlHelper.GetCategoryImageUrl(null, request);
        }


        public async Task<string> DeleteSubOneImageAsync(string id, string imageUrl, HttpRequest request)
        {
            var subOne = await _context.SubOnes.FindAsync(id)
                ?? throw new KeyNotFoundException("SubOne doesn't exist");

            if (string.IsNullOrWhiteSpace(subOne.Image) || subOne.Image != imageUrl)
                throw new InvalidOperationException("Image not found or doesn't match");

            DeleteCategoryImage(subOne.Image);

            subOne.Image = "no-image.png";
            await _context.SaveChangesAsync();

            var baseUrl = $"{request.Scheme}://{request.Host}";
            return UrlHelper.GetCategoryImageUrl(null, request);
        }


        public async Task<string> DeleteSubTwoImageAsync(string id, string imageUrl, HttpRequest request)
        {
            var subTwo = await _context.SubTwos.FindAsync(id)
                ?? throw new KeyNotFoundException("SubTwo doesn't exist");

            if (string.IsNullOrWhiteSpace(subTwo.Image) || subTwo.Image != imageUrl)
                throw new InvalidOperationException("Image not found or doesn't match");

            DeleteCategoryImage(subTwo.Image);

            subTwo.Image = "no-image.png";
            await _context.SaveChangesAsync();

            var baseUrl = $"{request.Scheme}://{request.Host}";
            return UrlHelper.GetCategoryImageUrl(null, request);
        }


        public async Task<string> DeleteSubThreeImageAsync(string id, string imageUrl, HttpRequest request)
        {
            var subThree = await _context.SubThrees.FindAsync(id)
                ?? throw new KeyNotFoundException("SubThree doesn't exist");

            if (string.IsNullOrWhiteSpace(subThree.Image) || subThree.Image != imageUrl)
                throw new InvalidOperationException("Image not found or doesn't match");

            DeleteCategoryImage(subThree.Image);

            subThree.Image = "no-image.png";
            await _context.SaveChangesAsync();

            var baseUrl = $"{request.Scheme}://{request.Host}";
            return UrlHelper.GetCategoryImageUrl(null, request);
        }


        // 📁 Helpers
        private async Task<string> UploadCategoryImageAsync(IFormFile file)
        {
            var fileName = FileHelper.GenerateImageFileName(file);
            var savePath = FileHelper.GetImageSavePath(_categoryImagesPath, fileName);
            await ImageHelper.CompressAndSaveAsync(file, savePath);
            return fileName;
        }

        private void DeleteCategoryImage(string? fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            var path = Path.Combine(_categoryImagesPath, fileName);
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}