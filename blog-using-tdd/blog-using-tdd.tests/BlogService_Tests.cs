using Xunit;
using blog_api.Controllers;
using Microsoft.AspNetCore.Mvc;
using blog_api;
using System.Collections.Generic;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace blog_using_tdd.tests
{
    public class BlogService_Tests
    {
        private readonly BlogController _blogController;
        private Mock<List<Post>> _mockPostsList;
       

        public BlogService_Tests()
        {
            _mockPostsList = new Mock<List<Post>>();
            _blogController = new BlogController(_mockPostsList.Object);
        }

        [Fact]
        public void GetTest_ReturnsListofPosts()
        {
            //arrange
            var mockPosts = new List<Post> {
                new Post{Title = "Tdd One"},
                new Post{Title = "Tdd and Bdd"}
            };

            _mockPostsList.Object.AddRange(mockPosts);

            //act
            var result = _blogController.Get();

            //assert
           var model = Assert.IsAssignableFrom<ActionResult<List<Post>>>(result);
            Assert.Equal(2, model.Value.Count);
        }

        [Fact]
        public void GetByIdTest_ReturnsNotFound_WhenIdNotProvided()
        {
            //act 
            var result = _blogController.GetById(null);

            //asert
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public void GetByIdTest_ReturnsNotFound_WhenPostDoesNotExit()
        {
            //arrange
            var post = new Post() { Id = new Guid("33704c4a-5b87-464c-bfb6-51971b4d18ad") };

            _mockPostsList.Object.SingleOrDefault(m => m.Id == post.Id);

            //act
            var result = _blogController.GetById(post.Id);

            //assert
            Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public void GetByIdTest_ReturnsSinglePost_WhenPostExist()
        {
            //arrange 
            var singleMockPost = new Post() { Id = new Guid("815accac-fd5b-478a-a9d6-f171a2f6ae7f") ,
                Title ="Learn TDD", Content ="Learning is fun"};

            _mockPostsList.Object.Add(singleMockPost);

            //act 
            var result = _blogController.GetById(singleMockPost.Id);

            //assert 
            var model = Assert.IsType<ActionResult<Post>>(result);
            Assert.Equal(singleMockPost, model.Value);
        }

        [Fact]
        public void AddTest_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            //arrange 
            var titleMissing = new Post() { Id = new Guid("815accac-fd5b-478a-a9d6-f171a2f6ae7f"), Content = "Learning is fun" };

            _blogController.ModelState.AddModelError("Title", "Title field is required");

            //act 
            var result = _blogController.Add(titleMissing);

            //assert 
            Assert.IsAssignableFrom<BadRequestObjectResult>(result);
        }

        [Fact]
        public void AddTest_ReturnsCreatedResponse_WhenValidObjectPassed()
        {
            //arrange
            var mockPost = new Post() { Content="Learning is fun",
                Title ="Begining to learn s the fun part about being learned" };

            //act 
            mockPost.Id = Guid.NewGuid();
            var result = _blogController.Add(mockPost);

            //assert
            Assert.IsAssignableFrom<CreatedAtActionResult>(result);
        }

        [Fact]
        public void AddTest_ReturnsResponseHasCreatedItem_WhenValidObjectPassed()
        {
            //arrange 
            var mockPost = new Post() { Id = new Guid("815accac-fd5b-478a-a9d6-f171a2f6ae7f"), Content="learning is fun",
                    Title = "Begining to learn is the fun part about being learned"};

            //act
            var result = _blogController.Add(mockPost) as CreatedAtActionResult;
            var item = result.Value as Post;

            //assert 
            Assert.IsType<Post>(item);
            Assert.Equal("learning is fun", item.Content);
        }

        [Fact]
        public void RemoveTest_ReturnsNotFound_WhenGuidNotExisting()
        {
            //arrange
            var notExistingGuid = Guid.NewGuid();

            //act
            var result = _blogController.Remove(notExistingGuid);

            //assert 
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void RemoveTest_ReturnsOkResult_WhenGuidIsExisting()
        {
            //arrange 
            var mockPost = new Post()
            {
                Id = new Guid("815accac-fd5b-478a-a9d6-f171a2f6ae7f"),
                Content = "learning is fun",
                Title = "Begining to learn is the fun part about being learned"
            };
            _mockPostsList.Object.Add(mockPost);


            //act
            var result = _blogController.Remove(mockPost.Id);

            //assert
            Assert.IsAssignableFrom<OkObjectResult>(result);
        }

        [Fact]
        public void RemoveTest_RemovesOneItem_WhenGuidIsExisting()
        {
            var mockPost = new List<Post>()
            {
                new Post(){Id = new Guid("815accac-fd5b-478a-a9d6-f171a2f6ae7f"), Title = "Learning One" },
                new Post(){Id = Guid.NewGuid(), Title = "Learning Two" },
                new Post(){Id = Guid.NewGuid(), Title = "Learning Three"}
            };
            _mockPostsList.Object.AddRange(mockPost);

            //act 
            _blogController.Remove(new Guid("815accac-fd5b-478a-a9d6-f171a2f6ae7f"));

            //assert
            Assert.Equal(2,_blogController.Get().Value.Count());
        }

        [Fact]
        public void UpdateTest_ReturnsNull_WhenIdAndPostAreNull()
        {
            //act
            var result = _blogController.Update(null,null);

            //assert
            Assert.IsAssignableFrom<NotFoundObjectResult>(result);
        }

        [Fact]
        public void UpdateTest_ReturnsNull_WhenIdIsNotNullAndPostIsNull()
        {
            //arrange 
            var mockPostId = Guid.NewGuid();

            //act 
            var result = _blogController.Update(mockPostId, null);

            //assert 
            Assert.IsAssignableFrom<NotFoundObjectResult>(result);
        }

        [Fact]
        public void UpdateTest_ReturnsNull_WhenIdIsNullAndPostIsNotNull()
        {
            //arrange 
            var mockPost = new Post() { Id = Guid.NewGuid(), Title = "lesson two" };

            //act
            var result = _blogController.Update(null, mockPost);

            //assert 
            Assert.IsAssignableFrom<NotFoundObjectResult>(result);
        }

        [Fact]
        public void UpdateTest_ReturnNotFoundResult_WhenIdNotExisting()
        {
            //arrange 
            var mockPost = new Post() { Id = Guid.NewGuid(), Title = "Lesson Three" };

            //act
            var result = _blogController.Update(mockPost.Id, mockPost);

            //assert
            Assert.IsAssignableFrom<NotFoundObjectResult>(result);
        }

        [Fact]
        public void UpdateTest_ReturnsOkResult_WhenIdIsPresent()
        {
            //arrange 
            var mockpost = new Post() { Id = new Guid("815accac-fd5b-478a-a9d6-f171a2f6ae7f"),
                Title = "Learning is fun", Content = "Learn well" };

            _mockPostsList.Object.Add(mockpost);

            //act
            var result = _blogController.Update(mockpost.Id, mockpost);

            //assert
            Assert.IsAssignableFrom<OkObjectResult>(result);
        }

        [Fact]
        public void UpdateTest_ReturnsNewItemAfterUpdate_WhenIdIsPresent()
        {
            //arrange 
            var mockpost = new Post()
            {
                Id = new Guid("815accac-fd5b-478a-a9d6-f171a2f6ae7f"),
                Title = "Learning is fun",
                Content = "Learn well"
            };
            _mockPostsList.Object.Add(mockpost);

            var mockpostToUpdate = new Post()
            {
                Title = "Learning is fun",
                Content = "Learn well again"
            };

            //act
            var result = _blogController.Update(mockpost.Id, mockpostToUpdate);

            //assert
            var model = Assert.IsAssignableFrom<OkObjectResult>(result);
            Assert.Equal(mockpostToUpdate.Content, _blogController.GetById(mockpost.Id).Value.Content);
        }
    }
}
