export default function ResultView({ data }) {
    if (!data) return null;

    return (
        <div style={{ padding: "20px" }}>
            <h2>Execution Plan Result</h2>

            <h3>Operators</h3>
            <ul>
                {data.logicalOpList.map((op, i) => (
                    <li key={i}>{op}</li>
                ))}
            </ul>

            <h3>Warnings</h3>
            <ul>
                {data.warnings.map((w, i) => (
                    <li key={i} style={{ color: "red" }}>{w}</li>
                ))}
            </ul>

            <h3>Metrics</h3>
            <p>Cost: {data.totalCost}</p>
            <p>Estimated Rows: {data.estimatedRows}</p>

           
        </div>
    );
}