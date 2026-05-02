$ErrorActionPreference = "Stop"

$inputJson = [Console]::In.ReadToEnd()
$lower = $inputJson.ToLowerInvariant()

$allowPatterns = @(
  "npm run build",
  "gh auth status",
  "gh run list",
  "gh run view",
  "gh run watch",
  "git status -sb",
  "git push",
  "az account show",
  "az deployment group what-if",
  "az deployment group create",
  "az deployment group show",
  "az role assignment list",
  "az role assignment create",
  "select-string",
  "dotnet build",
  "dotnet test"
)

$denyPatterns = @(
  "git reset --hard",
  "git checkout --",
  "az group delete"
)

$decision = "allow"
$reason = "Allowed by default policy for non-terminal tools."

foreach ($pattern in $denyPatterns) {
  if ($lower.Contains($pattern)) {
    $decision = "deny"
    $reason = "Blocked by pre-tool safety rule: $pattern"
    break
  }
}

if ($decision -eq "allow") {
  foreach ($pattern in $allowPatterns) {
    if ($lower.Contains($pattern)) {
      $decision = "allow"
      $reason = "Allowlisted command pattern: $pattern"
      break
    }
  }
}

if ($decision -eq "allow") {
  $isTerminalTool = $lower.Contains("run_in_terminal") -or $lower.Contains("send_to_terminal")
  if ($isTerminalTool) {
    $decision = "ask"
    $reason = "Terminal command not in allowlist."
  }
}

$output = @{
  hookSpecificOutput = @{
    hookEventName = "PreToolUse"
    permissionDecision = $decision
    permissionDecisionReason = $reason
  }
}

$output | ConvertTo-Json -Depth 4 -Compress
