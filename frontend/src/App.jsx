import { useState } from "react";
import { analyzeQuery } from "./api";
import ResultView from "./components/ResultView";

export default function App() {
    const [query, setQuery] = useState("");
    const [result, setResult] = useState(null);

    const handleSubmit = async () => {
        const data = await analyzeQuery(query);
        setResult(data);
    };

    return (
        <div style={{ padding: "20px" }}>
            <h1>Index Analyzer</h1>

            <textarea
                rows="4"
                cols="60"
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                placeholder="Enter SQL query..."
            />

            <br />

            <button onClick={handleSubmit}>
                Analyze
            </button>

            <ResultView data={result} />
        </div>
    );
}