#!/usr/bin/env bash
set -euo pipefail

echo "[Luban] Switching into tool directory..."
cd "$(git rev-parse --show-toplevel)/Tools/Luban/Tools"
echo "[Luban] Working dir: $(pwd)"

GEN_CLIENT="./Luban/Luban.dll"
CONF_ROOT="../DataTables"

echo "[Luban] Generating tables..."
if dotnet "$GEN_CLIENT" \
  -t client \
  -c cs-simple-json \
  -d json \
  --conf "$CONF_ROOT/luban.conf" \
  -x outputCodeDir=../../../Assets/Scripts/Generate/Luban \
  -x outputDataDir=../../../Assets/Res/Generate/Luban \
  "$@"; then
  echo "[Luban] Removing auxiliary build directories..."
  find ../../../Assets/Scripts/Generate/Luban -type d \( -name bin -o -name obj \) -prune -exec rm -rf {} +
  echo "[Luban] ✅ Generation succeeded."
else
  echo "[Luban] ❌ Generation failed." >&2
  exit 1
fi