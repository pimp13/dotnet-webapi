using System;

namespace MyFirstApi.Models;

public interface ISoftDeletable
{
  bool IsDeleted { get; set; }
}
