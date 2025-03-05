﻿namespace BudgedManager.Models.Entity;

public class HistoryDto
{
    public string Category { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Comment { get; set; }
}