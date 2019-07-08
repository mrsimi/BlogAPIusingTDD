using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace blog_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly List<Post> _posts;
        public BlogController(List<Post> posts)
        {
            _posts = posts;
        }
        public ActionResult<List<Post>> Get()
        {
            return _posts.ToList();
        }

        [HttpGet("{Id}")]
        public ActionResult<Post> GetById(Guid? id)
        {
            if (id==null)
            {
                return new NotFoundObjectResult("id not provided");
            }
            var singlePost = _posts.SingleOrDefault(m => m.Id == id);
            if (singlePost == null )
            {
                return new NotFoundObjectResult("Post Not found");
            }

            return singlePost;
        }

        [HttpPost]
        public ActionResult Add(Post post)
        {
            if (!ModelState.IsValid)
            {
                return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(ModelState);
            }
            post.Id = Guid.NewGuid();
            _posts.Add(post);

            return CreatedAtAction("Get", post);
        }

        [HttpDelete("{id}")]
        public ActionResult Remove(Guid id)
        {
            var postToDelete = _posts.SingleOrDefault(m => m.Id == id);
            if (postToDelete == null)
            {
                return new NotFoundObjectResult(postToDelete);
            }
            _posts.Remove(postToDelete);
            return new OkObjectResult(postToDelete.Title + " deleted");
        }
        

        [HttpPut("{id}")]
        public ActionResult Update(Guid? id, Post post)
        {
            if(id == null || post == null)
            {
                return new NotFoundObjectResult("One of Id or Post is missing");
            }
            var postToUpdate = _posts.FirstOrDefault(m => m.Id == id);
            _posts.Remove(postToUpdate);
            if(postToUpdate == null)
            {
                return new NotFoundObjectResult("The Post with the id is missing");
            }
            postToUpdate.Id = (Guid)id;
            postToUpdate.Content = post.Content;
            postToUpdate.Title = post.Title;
            _posts.Add(postToUpdate);
            return new OkObjectResult(postToUpdate);
        }
    }
}