using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Features.Orders.Dtos;

public record UpdateOrderStatusRequest(
    [Required(ErrorMessage = "Status is required")]
    [EnumDataType(typeof(OrderStatus), ErrorMessage = "Invalid status value")]
    string Status
    );