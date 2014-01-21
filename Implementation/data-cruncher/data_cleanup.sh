echo "Starting cleanup ..."
for f in data/*.csv
do
    echo "Fixing $f ..."
    sed -i .bak 's/\([0-9]\)\(,\)\([0-9]\)/\1.\3/g' "$f"
done

echo "Cleanup done!"
echo "Removing backup files..."
for f in data/*.bak
do
    echo "Removing sed backup file: $f"
    rm "$f"
done
echo "Done removing backup files!"
