#!/bin/bash
set -e

echo "=== Unity Git LFS cleanup & push script ==="

# Step 0: Safety backup
git branch backup/Demo-branch || true

# Step 1: Ensure correct remote
if ! git remote | grep -q origin; then
  echo "Adding remote origin..."
  git remote add origin https://github.com/EazyW96/Elliotte-Wideman-Unity-game-build.git
fi

# Step 2: Update .gitignore
cat >> .gitignore <<'EOF'

# Unity lighting/occlusion (large auto-generated)
**/[Ll]ightingData.asset
**/[Ll]ightmap*
**/OcclusionCullingData.asset
EOF
git add .gitignore
git commit -m "Ignore lighting/occlusion assets" || true

# Step 3: Remove tracked Library/Temp/UserSettings
git rm -r --cached GameUp/Library Space_Quest/Library GameUp/Temp Space_Quest/Temp \
  GameUp/UserSettings Space_Quest/UserSettings 2>/dev/null || true
git ls-files | grep -Ei '[Ll]ightingData\.asset$' | xargs -I{} git rm --cached "{}" 2>/dev/null || true
git commit -m "Untrack Unity caches & baked lighting" || true

# Step 4: Install git-filter-repo if missing
if ! command -v git-filter-repo &>/dev/null; then
  echo "Installing git-filter-repo via Homebrew..."
  brew install git-filter-repo
fi

# Step 5: Rewrite history (remove huge Unity files everywhere)
git filter-repo \
  --path-glob '*/Library/**' \
  --path-glob '*/Temp/**' \
  --path-glob '*/UserSettings/**' \
  --path-glob '**/[Ll]ightingData.asset' \
  --invert-paths --force

# Optional: remove old builds too
git filter-repo --path-glob '*/Build*/**' --invert-paths --force || true

# Step 6: Clean up garbage
git reflog expire --expire=now --all
git gc --prune=now --aggressive

# Step 7: Fetch & push clean history
git fetch origin Demo-branch || true
git push --force origin Demo-branch

echo "✅ Cleanup & force push complete!"
git lfs ls-files | head -n 15
echo "Verify that Library/, Temp/, and LightingData.asset are gone."

