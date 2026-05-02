#!/usr/bin/env bash
set -euo pipefail

input_json="$(cat)"
lower="$(printf '%s' "$input_json" | tr '[:upper:]' '[:lower:]')"

decision="allow"
reason="Allowed by default policy for non-terminal tools."

contains() {
  local haystack="$1"
  local needle="$2"
  case "$haystack" in
    *"$needle"*) return 0 ;;
    *) return 1 ;;
  esac
}

deny_patterns=(
  "git reset --hard"
  "git checkout --"
  "az group delete"
)

allow_patterns=(
  "gh auth status"
  "gh run list"
  "gh run view"
  "gh run watch"
  "git status -sb"
  "git push"
  "az account show"
  "az deployment group what-if"
  "az deployment group create"
  "az deployment group show"
  "az role assignment list"
  "az role assignment create"
  "select-string"
)

for p in "${deny_patterns[@]}"; do
  if contains "$lower" "$p"; then
    decision="deny"
    reason="Blocked by pre-tool safety rule: $p"
    break
  fi
done

if [[ "$decision" == "allow" ]]; then
  for p in "${allow_patterns[@]}"; do
    if contains "$lower" "$p"; then
      decision="allow"
      reason="Allowlisted command pattern: $p"
      break
    fi
  done
fi

if [[ "$decision" == "allow" ]]; then
  if contains "$lower" "run_in_terminal" || contains "$lower" "send_to_terminal"; then
    decision="ask"
    reason="Terminal command not in allowlist."
  fi
fi

printf '{"hookSpecificOutput":{"hookEventName":"PreToolUse","permissionDecision":"%s","permissionDecisionReason":"%s"}}\n' "$decision" "$reason"
