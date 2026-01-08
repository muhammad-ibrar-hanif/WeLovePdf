import { useState } from "react";
import type { DragEvent } from "react";
import axios from "axios";

export default function SplitPdf() {
    const [file, setFile] = useState<File | null>(null);
    const [loading, setLoading] = useState(false);

    const onDrop = (e: DragEvent<HTMLDivElement>) => {
        e.preventDefault();
        const dropped = Array.from(e.dataTransfer.files).find(
            (f) => f.type === "application/pdf"
        );
        if (dropped) setFile(dropped);
    };

    const handleSubmit = async () => {
        if (!file) return;

        const formData = new FormData();
        formData.append("file", file);

        setLoading(true);
        const res = await axios.post("/api/pdf/split", formData, {
            responseType: "blob",
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
            <h2 className="mb-3">Split PDF</h2>

            <div
                className="border border-2 border-dashed rounded p-5 text-center mb-3"
                onDragOver={(e) => e.preventDefault()}
                onDrop={onDrop}
            >
                <p className="mb-2">Drag & drop a PDF here</p>
                <input
                    type="file"
                    accept="application/pdf"
                    className="form-control"
                    onChange={(e) =>
                        setFile(e.target.files?.[0] ?? null)
                    }
                />
            </div>

            {file && (
                <div className="alert alert-secondary mb-3">
                    {file.name} — {Math.round(file.size / 1024)} KB
                </div>
            )}

            <button
                className="btn btn-primary"
                disabled={!file || loading}
                onClick={handleSubmit}
            >
                {loading ? "Splitting..." : "Split PDF"}
            </button>
        </div>
    );
}
