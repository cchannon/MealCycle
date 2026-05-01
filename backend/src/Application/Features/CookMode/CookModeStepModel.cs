namespace MealCycle.Application.Features.CookMode;

public sealed record CookModeStepModel(int StepIndex, string Instruction, bool IsCompleted);