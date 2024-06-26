﻿namespace OptimaJet.DataEngine.Queries.Filters;

public abstract class UnaryFilter : Filter
{
    protected UnaryFilter(IFilter operand) : base(new[] {operand})
    {
        Operand = operand;
    }
    
    public IFilter Operand { get; }
}