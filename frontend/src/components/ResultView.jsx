export default function ResultView({ data }) {
    if (!data) return null;

    return (
        <div style={{ padding: "20px" }}>
            <h2>Execution Plan Analysis</h2>

            <table border="1" cellPadding="10">
                <thead>
                    <tr>
                        <th>Logical Operator</th>
                        <th>Physical Operator</th>
                        <th>Index</th>
                        <th>Warning</th>
                        <th>Total Cost</th>
                        <th>Estimated Rows</th>
                        <th>Index Seek</th>
                        <th>Table Scan</th>
                        <th>Key Lookup</th>
                        <th>Join Type</th>
                    </tr>
                </thead>

                <tbody>
                    <tr>
                        <td>{data.logicalOpList.join(", ")}</td>

                        <td>{data.physicalOpList.join(", ")}</td>

                        <td>{data.indexes.join(", ")}</td>

                        <td style={{ color: "red" }}>
                            {data.warnings.join(", ")}
                        </td>

                        <td>{data.totalCost}</td>

                        <td>{data.estimatedRows}</td>

                        <td>{data.hasIndexSeek ? "Yes" : "No"}</td>

                        <td>{data.hasTableScan ? "Yes" : "No"}</td>

                        <td>{data.hasKeyLookup ? "Yes" : "No"}</td>

                        <td>{data.joinType || "None"}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    );
}