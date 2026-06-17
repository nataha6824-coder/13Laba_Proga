using _13Laba_Proga;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace _13Laba_Proga.Tests
{
    public class BooksApiTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public BooksApiTests(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetBooks_ReturnsSuccess()
        {
            using var client = _fixture.Factory.CreateClient();

            var response = await client.GetAsync("/books");

            Assert.True(response.IsSuccessStatusCode);
            var books = await response.Content.ReadFromJsonAsync<List<Book>>();
            Assert.NotNull(books);
        }

        [Fact]
        public async Task AddBook_ReturnsCreated()
        {
            using var client = _fixture.Factory.CreateClient();

            var newBook = new Book { Name = "Война и мир", Author = "Толстой", ReleaseDate = new DateTime(1869, 1, 1) };
            var response = await client.PostAsJsonAsync("/books", newBook);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var created = await response.Content.ReadFromJsonAsync<Book>();
            Assert.NotNull(created);
            Assert.Equal(newBook.Name, created.Name);
        }

        [Fact]
        public async Task UpdateBook_ReturnsOk()
        {
            using var client = _fixture.Factory.CreateClient();

            var newBook = new Book { Name = "Старое название", Author = "Старый автор", ReleaseDate = DateTime.Now };
            var postResponse = await client.PostAsJsonAsync("/books", newBook);
            var created = await postResponse.Content.ReadFromJsonAsync<Book>();

            var updatedBook = new Book { Name = "Новое название", Author = "Новый автор", ReleaseDate = DateTime.Now.AddDays(1) };
            var putResponse = await client.PutAsJsonAsync($"/books/{created.Id}", updatedBook);

            Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);
            var result = await putResponse.Content.ReadFromJsonAsync<Book>();
            Assert.Equal("Новое название", result.Name);
            Assert.Equal("Новый автор", result.Author);
        }

        [Fact]
        public async Task DeleteBook_ReturnsNoContent()
        {
            using var client = _fixture.Factory.CreateClient();

            var newBook = new Book { Name = "На удаление", Author = "Автор", ReleaseDate = DateTime.Now };
            var postResponse = await client.PostAsJsonAsync("/books", newBook);
            var created = await postResponse.Content.ReadFromJsonAsync<Book>();

            var deleteResponse = await client.DeleteAsync($"/books/{created.Id}");
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var getResponse = await client.GetAsync($"/books/{created.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteBook_NotFound_ReturnsNotFound()
        {
            using var client = _fixture.Factory.CreateClient();

            var response = await client.DeleteAsync("/books/99999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateBook_NotFound_ReturnsNotFound()
        {
            using var client = _fixture.Factory.CreateClient();

            var book = new Book { Name = "Тест", Author = "Тест", ReleaseDate = DateTime.Now };
            var response = await client.PutAsJsonAsync("/books/99999", book);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetBookById_ReturnsSuccess()
        {
            using var client = _fixture.Factory.CreateClient();

            var newBook = new Book { Name = "Тестовая книга", Author = "Тестовый автор", ReleaseDate = DateTime.Now };
            var postResponse = await client.PostAsJsonAsync("/books", newBook);
            var created = await postResponse.Content.ReadFromJsonAsync<Book>();

            var getResponse = await client.GetAsync($"/books/{created.Id}");
            Assert.True(getResponse.IsSuccessStatusCode);
            var book = await getResponse.Content.ReadFromJsonAsync<Book>();
            Assert.Equal(created.Id, book.Id);
            Assert.Equal(created.Name, book.Name);
        }
    }
}