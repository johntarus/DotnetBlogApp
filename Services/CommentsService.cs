using AutoMapper;
using BlogApp.Entities;
using BlogApp.Interfaces;
using BlogApp.Interfaces.Services;
using BlogApp.Models.Dtos;
using BlogApp.Models.Entities;

namespace BlogApp.Services;

public class CommentsService(ICommentRepository commentRepository, IMapper mapper) : ICommentsService
{
    public async Task<PaginatedList<CommentResponseDto>> GetCommentsAsync(int pageNumber, int pageSize)
    {
        var paginatedComments = await commentRepository.GetCommentsAsync(pageNumber, pageSize);
        var comments = mapper.Map<List<CommentResponseDto>>(paginatedComments.Items);
        return new PaginatedList<CommentResponseDto>(
            comments,
            paginatedComments.PageNumber,
            paginatedComments.PageSize,
            paginatedComments.TotalCount,
            paginatedComments.TotalPages
            );
    }

    public async Task<CommentResponseDto> CreateCommentAsync(CommentDto commentDto)
    {
        var comment = mapper.Map<Comment>(commentDto);

        var createdComment = await commentRepository.CreateCommentAsync(comment);

        return mapper.Map<CommentResponseDto>(createdComment);
    }

    public async Task<CommentResponseDto> GetCommentsByIdAsync(int id)
    {
        var comment = await commentRepository.GetCommentByIdAsync(id);
        if (comment == null) return null;
        return mapper.Map<CommentResponseDto>(comment);
    }

    public async Task<CommentResponseDto> UpdateCommentAsync(int id, UpdateCommentDto commentDto)
    {
        var commentToUpdate = await commentRepository.GetCommentByIdAsync(id);
        if (commentToUpdate == null) return null;
        mapper.Map(commentDto, commentToUpdate);
        commentToUpdate.UpdatedAt = DateTime.Now;
        commentToUpdate.IsEdited = true;
        var updatedComment = await commentRepository.UpdateCommentAsync(commentToUpdate);
        return mapper.Map<CommentResponseDto>(updatedComment);
    }

    public async Task<bool> DeleteCommentAsync(int id)
    {
        var comment = await commentRepository.GetCommentByIdAsync(id);
        if (comment == null) return false;
        await commentRepository.DeleteCommentAsync(comment);
        return true;
    }
}