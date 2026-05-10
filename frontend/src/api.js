import axios from "axios";

export const analyzeQuery = async (query) => {
    const response = await axios.post(
        "https://localhost:7193/api/Analyzer/execution-plan",
        {
            query: query
        }
    );

    return response.data;
};