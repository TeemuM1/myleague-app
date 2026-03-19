import { useEffect, useMemo, useState } from 'react';
import AdminPageTemplate from '../../../components/PageTemplate/AdminPageTemplate';
import LoadingSpinner from '../../../components/LoadingSpinner/LoadingSpinner';
import DOMPurify from 'dompurify';
import ReactQuill from 'react-quill';
import 'react-quill/dist/quill.snow.css';
import {
  getPageContent,
  updatePageContent,
  type PageContentResponse,
} from '../../../services/pageContentService';

type ViewMode = 'edit' | 'preview';

export default function RulesManagementPage() {
  const [mode, setMode] = useState<ViewMode>('edit');
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [loaded, setLoaded] = useState<PageContentResponse | null>(null);

  const [title, setTitle] = useState('Säännöt');
  const [contentHtml, setContentHtml] = useState('');

  useEffect(() => {
    let cancelled = false;

    const load = async () => {
      try {
        setIsLoading(true);
        setError(null);

        const data = await getPageContent('saannot');
        if (cancelled) return;

        setLoaded(data);
        setTitle(data.title || 'Säännöt');
        setContentHtml(data.contentHtml || '');
      } catch (e) {
        if (cancelled) return;
        const message = e instanceof Error ? e.message : 'Failed to load rules content.';
        setError(message);
        setLoaded(null);
        setTitle('Säännöt');
        setContentHtml('');
      } finally {
        if (!cancelled) setIsLoading(false);
      }
    };

    load();

    return () => {
      cancelled = true;
    };
  }, []);

  const sanitizedPreviewHtml = useMemo(() => {
    return DOMPurify.sanitize(contentHtml || '');
  }, [contentHtml]);

  const updatedInfo = useMemo(() => {
    if (!loaded?.updatedAt) return null;
    const date = new Date(loaded.updatedAt);
    if (Number.isNaN(date.getTime())) return null;
    return {
      updatedAt: date.toLocaleString('fi-FI'),
      lastModifiedBy: loaded.lastModifiedBy,
    };
  }, [loaded?.updatedAt, loaded?.lastModifiedBy]);

  const handleSave = async () => {
    try {
      setIsSaving(true);
      setError(null);

      const saved = await updatePageContent('saannot', {
        title: title.trim() || 'Säännöt',
        contentHtml: contentHtml,
      });

      setLoaded(saved);
      alert('Tallennettu.');
    } catch (e) {
      const message = e instanceof Error ? e.message : 'Failed to save rules content.';
      setError(message);
      alert('Tallennus epäonnistui.');
    } finally {
      setIsSaving(false);
    }
  };

  if (isLoading) {
    return (
      <AdminPageTemplate title="Säännöt">
        <div className="flex justify-center items-center min-h-screen">
          <LoadingSpinner text="Ladataan..." />
        </div>
      </AdminPageTemplate>
    );
  }

  return (
    <AdminPageTemplate title="Säännöt">
      <div className="max-w-6xl mx-auto space-y-6">
        <div className="bg-white border-b border-gray-200 sticky top-0 z-10">
          <div className="max-w-6xl mx-auto px-4 py-3">
            <div className="flex justify-between items-center">
              <div className="flex items-center gap-3">
                <span className="bg-blue-100 text-blue-800 text-sm font-medium px-3 py-1 rounded-full">
                  {mode === 'preview' ? 'Preview Mode' : 'Edit Mode'}
                </span>
                {updatedInfo && (
                  <span className="text-gray-600 text-sm">
                    Viimeksi muokattu: {updatedInfo.updatedAt}
                    {updatedInfo.lastModifiedBy ? ` (${updatedInfo.lastModifiedBy})` : ''}
                  </span>
                )}
              </div>

              <div className="flex gap-3">
                {mode === 'preview' ? (
                  <button
                    onClick={() => setMode('edit')}
                    className="px-4 py-2 bg-gray-600 text-white rounded-lg hover:bg-gray-700 transition-colors"
                  >
                    Muokkaa
                  </button>
                ) : (
                  <button
                    onClick={() => setMode('preview')}
                    className="px-4 py-2 bg-green-200 text-green-700 hover:bg-green-200 rounded-lg font-medium transition-colors"
                  >
                    Esikatselu
                  </button>
                )}

                <button
                  onClick={handleSave}
                  disabled={isSaving}
                  className={`px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors ${
                    isSaving ? 'opacity-50 cursor-not-allowed' : ''
                  }`}
                >
                  {isSaving ? 'Tallennetaan...' : 'Tallenna'}
                </button>
              </div>
            </div>
          </div>
        </div>

        {error && (
          <div className="bg-red-50 border border-red-200 text-red-800 rounded-lg p-4">
            {error}
          </div>
        )}

        {mode === 'preview' ? (
          <div className="bg-white rounded-lg shadow-sm border border-gray-200">
            <div className="p-6 space-y-4">
              <h2 className="text-xl font-semibold text-gray-900">{title || 'Säännöt'}</h2>
              {!sanitizedPreviewHtml ? (
                <p>Sääntöjä ei ole vielä tallennettu.</p>
              ) : (
                <div dangerouslySetInnerHTML={{ __html: sanitizedPreviewHtml }} />
              )}
            </div>
          </div>
        ) : (
          <div className="space-y-6">
            <div className="bg-white rounded-lg shadow-sm border border-gray-200">
              <div className="p-6">
                <label className="block text-sm font-medium text-gray-700 mb-2">Otsikko</label>
                <input
                  value={title}
                  onChange={(e) => setTitle(e.target.value)}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2"
                  placeholder="Säännöt"
                />
              </div>
            </div>

            <div className="bg-white rounded-lg shadow-sm border border-gray-200">
              <div className="p-6">
                <label className="block text-sm font-medium text-gray-700 mb-2">Sisältö</label>
                <div className="border rounded-lg border-gray-200">
                  <ReactQuill
                    theme="snow"
                    value={contentHtml}
                    onChange={setContentHtml}
                    modules={{
                      toolbar: [
                        [{ header: [1, 2, 3, false] }],
                        ['bold', 'italic', 'underline', 'strike'],
                        [{ list: 'ordered' }, { list: 'bullet' }],
                        ['link'],
                        ['clean'],
                      ],
                    }}
                  />
                </div>
              </div>
            </div>
          </div>
        )}
      </div>
    </AdminPageTemplate>
  );
}

