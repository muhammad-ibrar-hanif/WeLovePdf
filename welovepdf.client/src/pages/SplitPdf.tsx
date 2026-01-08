import { useState } from "react";
import axios from "axios";


export default function SplitPdf() {
    const [file, setFile] = useState<File | null>(null);
    const [loading, setLoading] = useState(false);


    const handleSubmit = async () => {
        if (!file) return;
        const formData = new FormData();
        formData.append("file", file);


        setLoading(true);
        const res = await axios.post("/api/pdf/split", formData, {
            responseType: "blob"
        });
        setLoading(false);


        const url = window.URL.createObjectURL(res.data);
        const a = document.createElement("a");
        a.href = url;
        a.download = "split-pages.zip";
        a.click();
    };


    return (
        <div className="container py-5">
            <h2>Split PDF</h2>
            <input
                type="file"
                accept="application/pdf"
                className="form-control mb-3"
                onChange={e => setFile(e.target.files?.[0] || null)}
            />
            <button
                className="btn btn-primary"
                disabled={!file || loading}
                onClick={handleSubmit}
            >
                {loading ? "Splitting..." : "Split"}
            </button>
        </div>
    );
}