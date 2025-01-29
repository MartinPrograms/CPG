using System.Numerics;
using CPG.Common;
using CPG.Common.Rendering;

namespace CPG.Interface;

public interface IGraphicsApi
{
    // Lifecycle
    void Init();
    void Shutdown();

    // Render Passes
    void BeginRenderPass(RenderPassInfo renderPassInfo);
    void EndRenderPass();

    // Pipeline Management
    IPipeline CreatePipeline(PipelineSettings settings);
    void BindPipeline(IPipeline pipeline);
    void DeletePipeline(IPipeline pipeline);

    // Resource Binding
    void BindVertexBuffers(IVertexBuffer[] buffers, uint[] offsets);
    void BindIndexBuffer(IIndexBuffer buffer, uint offset);
    void BindDescriptorSets(IDescriptorSet[] descriptorSets, uint firstSet = 0);

    // Drawing
    void Draw<V,I>(DrawCallSettings<V,I> drawInfo) where V : unmanaged where I : unmanaged;
    
    // Resource Creation
    IDescriptorSet CreateDescriptorSet(DescriptorSetSettings settings);
    ITexture CreateTexture(TextureSettings settings);
    IVertexBuffer CreateVertexBuffer<T>(VertexBufferSettings<T> settings) where T : unmanaged;
    IIndexBuffer CreateIndexBuffer<T>(IndexBufferSettings<T> settings) where T : unmanaged;
    IFrameBuffer CreateFrameBuffer(FrameBufferSettings settings);

    // Resource Deletion
    void DeleteDescriptorSet(IDescriptorSet descriptorSet);
    void DeleteTexture(ITexture texture);
    void DeleteVertexBuffer(IVertexBuffer buffer);
    void DeleteIndexBuffer(IIndexBuffer buffer);
    void DeleteFrameBuffer(IFrameBuffer buffer);

    // Data Updates
    void UpdateBuffer<T>(GpuBuffer<T> buffer, T[] data) where T : unmanaged;
    
    GraphicsError? GetLastError();
}

