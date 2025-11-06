#!/usr/bin/env bash
set -euo pipefail

# ====== CONFIG ======
REMOTE_URL="https://github.com/EazyW96/Elliotte-Wideman-Unity-game-build.git"
BRANCH="Demo-branch"
# ====================

echo "=== Unity Git LFS cleanup (one-time) ==="

# Safety: ensure we’re in a git repo
git rev-parse --show-toplevel >/dev/null

# Safety backup (idempotent)
git branch "backup/${BRANCH}" 2>/dev/null || true

# Ensure remote 'origin' exists (will be re-added again later if filter-repo removes it)
if ! git remote | grep -q '^origin$'; then
  git remote add origin "$REMOTE_URL"
fi

# Ensure we’re on the target branch
CURRENT_BRANCH=$(git rev-parse --abbrev-ref HEAD)
if [[ "$CURRENT_BRANCH" != "$BRANCH" ]]; then
  echo "Switching to $BRANCH"
  git checkout -B "$BRANCH"
fi

# --- .gitignore (append if not already present) ---
if ! grep -q 'LightingData.asset' .gitignore 2>/dev/null; then
  cat >> .gitignore <<'EOF'

# --- Unity heavy generated content (ignore) ---
[Ll]ibrary/
[Tt]emp/
[Oo]bj/
[Bb]uild*/
[Ll]ogs/
UserSettings/
**/Bee/
**/BurstCache/
**/Artifacts/
**/[Ll]ightingData.asset
**/[Ll]ightmap*
**/OcclusionCullingData.asset
[Mm]emoryCaptures/
[Cc]rashReports/
[Ss]ysinfo/
EOF
  git add .gitignore
  git commit -m "Add Unity ignore rules for cache/baked assets" || true
fi

# --- LFS setup ---
git lfs install --local || true

# Track common Unity binary assets with LFS (append only if missing)
ensure_lfs_track() {
  local pattern="$1"
  if ! git check-attr --all "$pattern" 2>/dev/null | grep -q 'filter=lfs'; then
    git lfs track "$pattern"
  fi
}
ensure_lfs_track "*.png"
ensure_lfs_track "*.jpg"
ensure_lfs_track "*.jpeg"
ensure_lfs_track "*.psd"
ensure_lfs_track "*.tga"
ensure_lfs_track "*.fbx"
ensure_lfs_track "*.wav"
ensure_lfs_track "*.mp3"
ensure_lfs_track "*.aiff"
ensure_lfs_track "*.ogg"
ensure_lfs_track "*.mp4"
ensure_lfs_track "*.mov"
ensure_lfs_track "*.prefab"
ensure_lfs_track "*.anim"
ensure_lfs_track "*.controller"
ensure_lfs_track "*.asset"
ensure_lfs_track "*.mat"

# Commit .gitattributes if changed
git add .gitattributes 2>/dev/null || true
git commit -m "Configure Git LFS for common Unity binaries" || true

# --- Stop tracking heavy generated stuff already added in the past ---
# Cache folders (ignore errors if paths don’t exist)
git rm -r --cached --ignore-unmatch \
  */Library */Temp */UserSettings 2>/dev/null || true

# Baked lighting / occlusion
git ls-files | grep -Ei '(^|/)(LightingData\.asset|Lightmap[^/]*|OcclusionCullingData\.asset)$' \
  | xargs -I{} git rm --cached --ignore-unmatch "{}" 2>/dev/null || true

git commit -m "Untrack Unity caches and baked lighting/occlusion" || true

# --- Install git-filter-repo if missing (macOS Homebrew) ---
if ! command -v git-filter-repo >/dev/null 2>&1; then
  echo "Installing git-filter-repo (brew)…"
  brew install git-filter-repo
fi

# --- Rewrite history to remove heavy generated files everywhere ---
git filter-repo \
  --path-glob '*/Library/**' \
  --path-glob '*/Temp/**' \
  --path-glob '*/UserSettings/**' \
  --path-glob '**/[Ll]ightingData.asset' \
  --path-glob '**/[Ll]ightmap*' \
  --path-glob '**/OcclusionCullingData.asset' \
  --invert-paths --force

# OPTIONAL: strip historical Build folders too (safe)
git filter-repo --path-glob '*/Build*/**' --invert-paths --force || true

# After filter-repo, remotes are removed on purpose — restore origin
if ! git remote | grep -q '^origin$'; then
  git remote add origin "$REMOTE_URL"
fi

# --- Garbage collect & shrink repo ---
git reflog expire --expire=now --all
git gc --prune=now --aggressive

# --- Push cleaned history ---
git fetch origin "$BRANCH" || true

# Prefer a guarded force with explicit lease (safe if remote moved since fetch)
REMOTE_SHA=$(git rev-parse --verify "origin/${BRANCH}" 2>/dev/null || echo "")
if [[ -n "$REMOTE_SHA" ]]; then
  echo "Force-pushing with lease @ $REMOTE_SHA"
  git push --force-with-lease=refs/heads/${BRANCH}:${REMOTE_SHA} origin "$BRANCH" || {
    echo "Lease failed; falling back to force."
    git push --force origin "$BRANCH"
  }
else
  echo "No remote tip found; pushing new branch."
  git push --force origin "$BRANCH" || git push -u origin "$BRANCH"
fi

echo " Done. Sanity checks:"
git ls-files | grep -E '(^|/)(Library|Temp|UserSettings)/|LightingData\.asset' || echo "OK: no generated files tracked"
git lfs ls-files | head -n 20 || true
git count-objects -vH | sed -n '1,8p'

